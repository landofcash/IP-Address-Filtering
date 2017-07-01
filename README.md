[![Build status](https://ci.appveyor.com/api/projects/status/github/dejanstojanovic/IP-Address-Filtering?branch=master&svg=true)](https://ci.appveyor.com/project/dejanstojanovic/ip-address-filtering/branch/master)

# IP-Address-Filtering
Lightweight C# ASP.NET MVC address filtering library

The library allows validating IP address against:
* Single IP address
* List of multiple IP addresses
* Single IP address range
* Multiple IP address ranges
* Roles+IP list stored in updateable static class

## How to use

```cs
	[IPAddressFilter("::1", IPAddressFilteringAction.Allow)]
	public ActionResult TestLocal()
	{
	    ViewBag.Message = "Local test page.";

	    return View();
	}
	[IPAddressFilter("192.168.0.100", IPAddressFilteringAction.Allow)]
	public ActionResult TestRemote()
	{
	    ViewBag.Message = "Remote test page.";

	    return View();
	}
```

## Roles with IP list

First need to fill roles + IP list, do that on app start
```cs
	var rolesWithIpList = new Dictionary<string, List<string>>()
	{
		{ "admin", new List<string>() {"192.168.10.100"}},
                { "user", new List<string>() {"192.168.10.200", "192.168.10.201", "192.168.10.202"}},
                { "local", new List<string>() { "127.0.0.1", "::1"}},
	};
	RolesContainer.UpdateRolesList(rolesWithIpList);
```

In controller mark methods with IPAddressRoleFilter attribute and specify roles that are allowed.

```cs
	[IPAddressRoleFilter("admin,local")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
```