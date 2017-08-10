using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VU.BLL;
using VU.UIL.LMS;

namespace VU.UIL
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SnapshotManager snapshotManager = new SnapshotManager();
            snapshotManager.Initialize();
            VideoSharingManager videoSharingManager = new VideoSharingManager(Server.MapPath(@"~/"), 10, 0);

        }
    }
}