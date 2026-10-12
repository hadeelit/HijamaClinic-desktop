using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Licensing;
using System.Windows;

namespace HijamaClinicManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Register syncfusion Claim License Key for app 
        public App()
        {
            SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHRqVVhlWFpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF5jQX5UdkJmX3xednBVQA==;Mgo+DSMBPh8sVXJ0S0J+XE9BclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TfkVkWXhbdndVQGVVVw==;ORg4AjUWIQA/Gnt2VVhkQlFac1tJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdk1jXn9ZdXJWRmZbV0I=;MjYyODExMUAzMjMwMmUzNDJlMzBlR3VlNlExMjU5UHllSUh5Yk03UUhka3JmSUpMR0h4WnUvQ3BrRVZkUjZJPQ==;MjYyODExMkAzMjMwMmUzNDJlMzBYbnBDdTZXQTF4U2t6TGZ4MkdHUExzTDMwSVN0ZllaVlJMSE9wUko4NlBnPQ==;NRAiBiAaIQQuGjN/V0Z+WE9EaFpGVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdUVrWXhed3FTRWBbV0Vw;MjYyODExNEAzMjMwMmUzNDJlMzBKemxuQTg4OEt1cTBtU2x0YjlnYUROeU9ZK3ZNRmcwZ05iY0tWN0p1NlVBPQ==;MjYyODExNUAzMjMwMmUzNDJlMzBNTGN1UzV1eGsxV0JmbS9JbjQ2RmlpL0JQS1BVOGlIdVcwK0xtOEhiWlM0PQ==;Mgo+DSMBMAY9C3t2VVhkQlFac1tJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdk1jXn9ZdXJWR2BcU0I=;MjYyODExN0AzMjMwMmUzNDJlMzBkNmhNL0ZYdlVza2dRSmdXalRwaTRrN2pWMVZnQzgrOWRPcTdlUWhmZWJnPQ==;MjYyODExOEAzMjMwMmUzNDJlMzBFempreUs4TnFvMXVUT3d4dHMrVHJvQTBDL0NQQ1RPOWMxb0JBVWtLVTVZPQ==;MjYyODExOUAzMjMwMmUzNDJlMzBKemxuQTg4OEt1cTBtU2x0YjlnYUROeU9ZK3ZNRmcwZ05iY0tWN0p1NlVBPQ==");
        }
    }
}
