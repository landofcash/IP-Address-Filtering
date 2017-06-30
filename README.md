[![Build status](https://ci.appveyor.com/api/projects/status/github/dejanstojanovic/IP-Address-Filtering?branch=master&svg=true)](https://ci.appveyor.com/project/dejanstojanovic/ip-address-filtering/branch/master)

# IP-Address-Filtering
Lightweight C# ASP.NET MVC and Web API IP address filtering library

The library allows validating IP address against:
* Single IP address
* List of multiple IP addresses
* Single IP address range
* Multiple IP address ranges
* Roles+IP list stored in updateable static class

## How to use

```cs
    public class DataController : ApiController
    {
        [HttpGet]
        [Route("api/data/{recordId}")]
        [IPAddressFilter("94.201.50.212", IPAddressFilteringAction.Restrict)]
        public HttpResponseMessage GetData(int recordId)
        {
            /* Create response logic here */
            return this.Request.CreateResponse<object>(HttpStatusCode.OK, new object());
        }
    }
```

##Roles with IP list

First need to fill roles + IP list, do that on app start
```cs
	var rolesWithIpList = new Dictionary<string, List<string>>()
	{
		{ "admin", new List<string>() {"192.168.10.100"}},
		{ "user", new List<string>() {"192.168.10.200", "192.168.10.201", "192.168.10.202"}}
	};
	RolesContainer.UpdateRolesList(rolesWithIpList);
```

```cs
    public class DataController : ApiController
    {
        [HttpGet]
        [Route("api/data/{recordId}")]
        [IPAddressFilter("admins, users", IPAddressFilteringAction.Allow)]
        public HttpResponseMessage GetData(int recordId)
        {
            /* Create response logic here */
            return this.Request.CreateResponse<object>(HttpStatusCode.OK, new object());
        }
    }
```