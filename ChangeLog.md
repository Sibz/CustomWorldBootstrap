**v1.2.2 Changes**
* Updated project unity version
* Updated project packages
* Refactored internal PerWorldPostInitialization code

**v1.2.1 Changes**
* Fixed null reference issue when not using CreateInWorld attribute but specifying systems via the CustomIncludeQuery

**v1.2.0 Changes**
* Added option for including systems based on a custom query
See **World Options** below for more information
* Full details in [Pull request #34](pull/34)

**v1.1.3 Changes**
* Full details in [Pull request #31](pull/31)

**v1.1.2 Changes**
* Fixed an issue where WorldOptions was not initialized

**v1.1.1 Changes**
* WorldOption is now located in CustomWorldBootstrap class.
*Previously it was erronously put in global and CustomWorldBootstrapInternal namespaces. If you have referenced it in these namespaces it will now be marked as obsolete*
* Code tidy up done cleaning up unused usings and unnecessary whitespace
* Some small refactoring changes