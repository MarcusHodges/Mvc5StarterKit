# Izenda Mvc5Starterkit

## Overview
The MVC5 Starterkit showcases how to embed Izenda in a MVC5 application.

 :warning: **The MVC Kit is designed for demonstration purposes and should not be used as an “as-is” fully-integrated solution. You can use the kit for reference or a baseline but ensure that security and customization meet the standards of your company.**
 
## Getting Started 
 Download the most recent version (https://downloads.izenda.com/latest/) of the API and EmbeddedUI and copy the following:

All files & folders in API\bin -> Mvc5StarterKit\IzendaReferences

API\Content -> Mvc5StarterKit\IzendaResources
API\EmailTemplates -> Mvc5StarterKit\IzendaResources
API\Export -> Mvc5StarterKit\IzendaResources

EmbeddedUI -> Mvc5StarterKit\Scripts\izenda

Open SSMS
- Create a new database named Retail
- Run script located at Mvc5StarterKit\SQLScript\RetailDbScript.sql

Run Application and login by System Admin Account below:
- Tenant : System
- Username: IzendaAdmin@system.com
- Password: Izenda@123

After start up and logging in as Admin Go to Settings on top Nav
- Add Izenda License Key and Token (leave Izenda Configuration Database Connection string set this is using an MDF file included in this kit)
- Go to Data Setup > Connection String  add the connection string to the Retail database created above for system and all tenants and move tables/views to visible
   More Info here:<a href="https://www.izenda.com/docs/ui/doc_connection_string.html?highlight=connection%20string" /> How To Set Up A Connection String</a>
   
- In Data Setup > Advanced Settings > Security Set the Tenant Field to [CustomerID] (please ensure you use the brackets) for each Tenant / CustomerID in the Retail Database are the tenants  (DELDG/NATWR/RETCL) and this will filter data
   based on the current user's TenantID
   More Info here: <a href = "https://www.izenda.com/docs/ui/doc_advanced_settings.html?highlight=set%20tenant%20field#update-settings-in-security-tenant-group"/> Updating settings in Performance, Security/Additive Fields and Others group </a> 

- For each Role in the tenant set datamodel access (what tables / views each role can access)
  More Info here: <a href = "https://www.izenda.com/wiki7/ui/doc_role_setup.html?highlight=role%20setup" /> How To Set Up A Role</a>

### Tenants, Users, and Roles
For each Tenant the following users / roles are configured all use the same password: Izenda@123

Tenant: DELDG <br />
User: employee@deldg.com        Role: employee <br />
User: manager@deldg.com         Role: manager <br />
User: vp@deldg.com              Role: VP <br />

Tenant: NATWR <br />
User: employee@natwr.com        Role: employee <br />
User: manager@natwr.com         Role: manager <br />
User: VP@natwr.com              Role: VP <br />

Tenant: RETCL <br />
User: employee@retcl.com         		Role: employee <br />
User: manager@retcl.com    								Role: manager <br />
User: vp@retcl.com         								Role: VP <br />

When registering a new user in this sample all users are hardcoded to the manager role here:
Mvc5StarterKit\Controllers\AccountController.cs (Line 177)	

Please review the following file:
mvc5starterkit\mvc5starterkit\izendaboundary\customadhocreport.cs
This is where you can find samples for:
Hidden Filters
Filter Dropdown Overrides
See more information here: <a href="https://www.izenda.com/wiki7/dev/ref_iadhocextension.html?highlight=iadhocextension" /> All About IAdhocExtension, Hidden Filters, and Filter Dropdown Overrides </a>


The CSS can be configured per tenant and an example is provided see below:
This is configured here ~\mvc5starterkit\Mvc5StarterKit\Views\Shared\_Layout.cshtml
And folder structures are located here ~\mvc5starterkit\Mvc5StarterKit\Content

## SQL Scripts

The MVC5Starterkit is pre-set to use locally installed instances of SQL LocalDB (mdf) for the Izenda database and the MVC5Starterkit database. You can configure the kit to use full-fledged sql server instances by following the instructions below.

### Creating the Izenda database
This is the database for the Izenda configuration. It contains report definitions, dashboards,etc.
- Create a database named 'IzendaMVC'. You may use any name of your choosing, just be sure to modify the script below to use the new database name.
- Download and execute the <a href="https://github.com/Izenda7Series/Mvc5StarterKit/blob/master/SQLScript/MSSQL/IzendaMvc.sql">IzendaMVC.sql</a> script.  
- Modify the <a href="https://github.com/Izenda7Series/Mvc5StarterKit/blob/master/Mvc5StarterKit/izendadb.config">izendadb.config</a> file with a valid connection string to this new database.

```json
{"ServerTypeId":"572bd576-8c92-4901-ab2a-b16e38144813","ServerTypeName":"[MSSQL] SQLServer","ConnectionString":"[your connection string here]","ConnectionId":"00000000-0000-0000-0000-000000000000"}

``` 

### Creating the Mvc5StarterKit database
This is the database for the Mvc5 application. It contains the users, roles, tenants used to login.
- Create a database named 'Mvc5StarterKit'. You may use any name of your choosing, just be sure to modify the script below to use the new database name.
- Download the <a href="https://github.com/Izenda7Series/Mvc5StarterKit/blob/master/SQLScript/MSSQL/Mvc5StarterKit.sql">Mvc5StarterKit.sql</a> script.
- Modify the <a  href="https://github.com/Izenda7Series/Mvc5StarterKit/blob/master/Mvc5StarterKit/Web.config">web.config (Line 86)</a> file with a valid connection string to this new database.

```xml
  <connectionStrings>
    <add name="DefaultConnection" connectionString="[your connection string here]" providerName="System.Data.SqlClient" />
  </connectionStrings>
``` 
