using System;
using System.Collections.Generic;
using System.Linq;
using AbpInsight.Utils;
using JetBrains.ReSharper.Feature.Services.Web.AspRouteTemplates.EndpointsProvider.AspNetHttpEndpoints;
using JetBrains.ReSharper.Feature.Services.Web.AspRouteTemplates.EndpointsProvider.AspNetHttpEndpoints.AttributeRouting;
using JetBrains.ReSharper.Feature.Services.Web.AspRouteTemplates.EndpointsProvider.AspNetHttpEndpoints.RouteSources;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace AbpInsight.VoloAbp.Mvc;

public static class ConventionalControllerUtil
{
    private static readonly Dictionary<string, string[]> ConventionalPrefixes = new()
    {
        { "GET", ["GetList", "GetAll", "Get"] },
        { "PUT", ["Put", "Update"] },
        { "DELETE", ["Delete", "Remove"] },
        { "POST", ["Create", "Add", "Insert", "Post"] },
        { "PATCH", ["Patch"] }
    };

    private static readonly string[] CommonPostfixes = ["AppService", "ApplicationService", "Service"];


    public static IEnumerable<AspNetHttpEndpoint> GetControllerEndpoints(IClass clazz, IPsiModule psiModule)
    {
        if (!IsController(clazz))
            yield break;

        foreach (var method in GetControllerActions(clazz, psiModule))
        {
            foreach (var endpoint in GetActionEndpoints(method, clazz, psiModule))
            {
                yield return endpoint;
            }
        }
    }

    private static string GetConventionalVerbForMethodName(string methodName)
    {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var conventionalPrefix in ConventionalPrefixes)
        {
            if (conventionalPrefix.Value.Any(prefix => methodName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            {
                return conventionalPrefix.Key;
            }
        }

        return "POST";
    }

    public static bool IsController(IClass clazz)
    {
        if (clazz.GetAccessRights() != AccessRights.PUBLIC || clazz.IsAbstract || clazz.HasTypeParameters())
            return false;

        return clazz.DerivesFrom(KnownTypes.IRemoteService);
    }

    private static IEnumerable<IMethod> GetControllerActions(IClass clazz, IPsiModule psiModule)
    {
        if (!psiModule.IsValid() || !clazz.IsValid())
            yield break;

        foreach (var method in clazz.Methods)
        {
            if (method.GetAccessRights() != AccessRights.PUBLIC || !method.IsAbstract)
            {
                yield return method;
            }
        }
    }

    private static IEnumerable<AspNetHttpEndpoint> GetActionEndpoints(IMethod method, IClass clazz, IPsiModule psiModule)
    {
        if (clazz.IsValid() && method.GetAccessRights() == AccessRights.PUBLIC && method is { IsAbstract: false, IsStatic: false })
        {
            var routeSubstitutions = RouteValuesUtil.GetAvailableRouteValues(psiModule, clazz, method).CreateAllPossibleSubstitutionDatum().ToArray();


            var routingAttributesProvider = RoutingAttributesProvider.GetInstance(psiModule);

            var routingAttributes = routingAttributesProvider.GetRoutingAttributes(method);

            if (routingAttributes.Any())
            {
                foreach (var routingAttribute in routingAttributes)
                {
                    foreach (var verb in routingAttribute.Verbs)
                    {
                        yield return new AspNetHttpEndpoint(psiModule, clazz, method, verb,
                            [new RouteTemplateProvider(routingAttribute.Template, psiModule)], routeSubstitutions[0]);
                    }
                }
            }
            else
            {
                var template = $"api/app/{clazz.ShortName.RemovePostfix(CommonPostfixes).ToKebabCase()}";


                if (method.Parameters.Any(it => it.ShortName == "id" && it.Type.IsSimplePredefined()))
                {
                    template += "/{id}";
                }

                var httpVerb = HttpVerb.Exact(GetConventionalVerbForMethodName(method.ShortName));


                var actionName = method.ShortName;

                if (ConventionalPrefixes.TryGetValue(httpVerb.ToString(), out var prefixes))
                {
                    actionName = actionName.RemovePrefix(prefixes).RemovePostfix("Async").ToKebabCase();
                }

                template += $"/{actionName}";

                var templateProvider = new RouteTemplateProvider(template, psiModule);

                yield return new AspNetHttpEndpoint(psiModule, clazz, method, httpVerb, [templateProvider], routeSubstitutions[0]);
            }
        }
    }


    private class RouteTemplateProvider(string? template, IPsiModule psiModule) : IRouteTemplateProvider
    {
        public string? Template { get; } = template;

        public IPsiModule PsiModule { get; } = psiModule;

        public RouteTemplateSource TemplateSource => RouteTemplateSource.RoutingConvention;


        public int CompareTo(IRouteTemplateProvider other)
        {
            return 1;
        }
    }
}