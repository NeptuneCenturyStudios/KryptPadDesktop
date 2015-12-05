using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadDesktop.Models
{
    class StartPageViewModel : BaseModel
    {
        #region Properties

            public Command Test { get; protected set; }

        #endregion

        public StartPageViewModel() {
            Test = new Command((p) => {



            });
        }

    }
}
