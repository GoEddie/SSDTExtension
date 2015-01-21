SSDTExtension
=============
An extension for ssdt to help with stuff like deploying single files so the red-green cycle of tdd can be done quickly (rather than building and deploying the whole project every change)



All this does is:


1) Right click on a stored procedure in an SSDT project and choose "Generate Deploy Script" - what this does is create a .sql file in a deployment folder which does a drop if the proc exists and then create.

2) Right click on a stored procedure in an SSDT project and choose "Deploy File" - this will deploy a single file to the configured sql server. If your writing tests or changes to the proc and you want to test it without building and deploying then use this. If you have any SqlCmd variables then they are replaced with the default project ones.

3) On the project menu, you can choose "Configure Sql Tdd Extender" which allows you to configure the connection string to be used deploy files and the deployment folder to put scripts.

The config is stored in the root of the SSDT project called rather grandly "SSDTExtender.config"





The reason for this is that it takes ages to build and deploy singel files as part of the tdd t-sql approach and this makes my life easier. Please fork and improve.


To install, either download the code and run the visual studio project (you will need the 2013 visual studio sdk) - or download from https://github.com/GoEddie/SSDTExtension/blob/master/download/0.4.2/SqlServerTddHelper.vsix?raw=true  and run it on a machine with visual studio 2013 installed. 


