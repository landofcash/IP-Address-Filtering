[![Build status](https://ci.appveyor.com/api/projects/status/github/dejanstojanovic/IP-Address-Filtering?branch=master&svg=true)](https://ci.appveyor.com/project/dejanstojanovic/ip-address-filtering/branch/master)

# IP-Address-Filtering
Lightweight C# ASP.NET MVC address filtering library

The library allows validating IP address against:
* Single IP address
* List of multiple IP addresses
* Single IP address range
* Multiple IP address ranges
* Roles+IP list stored in updateable static class

## How to use with the IP address hardcoded into attribute

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

First need to fill roles + IP list, do that on app start, use "0.0.0.0" to allow any IP
```cs
	var rolesWithIpList = new Dictionary<string, List<string>>()
	{
		{ "admin", new List<string>() {"192.168.10.100"}},
                { "user", new List<string>() {"192.168.10.200", "192.168.10.201", "192.168.10.202"}},

	};
	RolesContainer.UpdateRolesList(rolesWithIpList);
```



In controller mark methods with IPAddressRoleFilter attribute and specify roles that are allowed.
Note that there are two predefined roles: "local" ("127.0.0.1", "::1") and "any" ("0.0.0.0")

```cs
	[IPAddressRoleFilter("admin,local")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
```

Also the role named "global" is added by default to all IPAddressRoleFilter attributes
This role allows to enable/disable IPs for all IPAddressRoleFilter attributes.

```cs
	[IPAddressRoleFilter("admin,local")]
	// same as
	[IPAddressRoleFilter("global,admin,local")]
```