# FileUpdateChecker

C# Project that checks for changes in a file in a given resolution.  

Flow -  
1. run first on the desired file, and a .state file will be created containing md5 checksums for each 
    'resolution' sized interval.  
2. second run will process the file again, and check each interval in comparison to the one saved in the
    .state file, if a change is found the relevant intervals are returned.  
3. the .state file is changed according to the new md5, and so on.  

This process may run on extremely large files, given that the resolution parameter is smaller
than the machine's available memory.
