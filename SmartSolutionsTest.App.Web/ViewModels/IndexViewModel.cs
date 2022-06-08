using System.Collections;
using System.Collections.Generic;

namespace SmartSolutionsTest.App.Web.ViewModels
{
    public class IndexViewModel
    {
        public bool ShowErrors { get; set; } = false;

        public List<OperationViewModel> Operations { get; set; } = new List<OperationViewModel>();
    }
}
