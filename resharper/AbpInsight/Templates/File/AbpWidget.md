---
guid: E342F586-1A6C-4E66-938B-5345236042F1
image: AbpInsight
type: File
reformat: True
shortenReferences: True
categories: AbpInsight
customProperties: Extension=cs, FileName=AbpWidget, ValidateFileName=True
scopes: NotInAnyProject
uitag: Abp Widget
parameterOrder: HEADER, (CLASS), (NAMESPACE)
HEADER-expression: fileheader()
CLASS-expression: getAlphaNumericFileNameWithoutExtension()
NAMESPACE-expression: fileDefaultNamespace()
---

# Abp Widget

```
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

$HEADER$namespace $NAMESPACE$ {

  [Widget]
  public class $CLASS$ : AbpViewComponent {
    public IViewComponentResult Invoke()
    {
      return View();
    }
    $END$
  }
}
```