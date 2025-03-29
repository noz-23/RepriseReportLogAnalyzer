/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using System.ComponentModel;

namespace RepriseReportLogAnalyzer.Enums;

internal enum SelectData : long
{
    [Description("There is some kind of Exclusion")]
    ECLUSION = -1,

    [Description("There are All without exclusions")]
    ALL = 0,
}

internal enum AnalysisGroup
{
    [Description("None")]
    NONE = 0,
    [Description("User")]
    USER = 1,
    [Description("Host")]
    HOST = 2,
    [Description("User@Host")]
    USER_HOST = 3,
}

internal enum LogFormat
{
    [Description("No difference in format")]
    NONE,

    [Description("Small Format")]
    SMALL,

    [Description("Standard Format")]
    STANDARD,

    [Description("Detailed Format")]
    DETAILED
}

internal enum ReservationType
{
    CREATE,
    REMOVE
}

internal enum LicenseTemporaryType
{
    CREATE,
    REMOVE,
    RESTART,
    EXPIRED,
}
internal enum SwitchType
{
    TO,
    FROM,
}

internal enum StatusValue
{
    // check-in why value Reason
    Normal = 1,     //“Normal” check-in by application
    Automatic = 2,   //Application exited, automatic check-in
    Utilit = 3, //License removed by (rlmremove) utility
    Timeout = 4,    //License removed by server after timeout
    Hold = 5,   //License hold/minimum checkout period expired
    Dequeue = 6,    //Client requested license dequeue
    HostId = 7, //Portable hostid removed
    BackUp = 8, //Failed host back up
    Lost = 9,   //Server lost its transferred licenses
    Meter = 10, //Meter ran out of count during a periodic decrement
    heartbeat11,    //Client failed to send heartbeat within “promise” interval.
    Expired = 12,   //Temporary License expired (RLM Cloud only)
    Returned = 13,	//Temporary license returned (RLM Cloud only)

    // rlm_stat() returns general RLM_HANDLE errors. These are:
    RLM_EH_NOHANDLE = -101, //No handle supplied to call.
    RLM_EH_READ_NOLICENSE = -102,   //Can’t read license data.
    RLM_EH_NET_INIT = -103, //Network (msg_init()) error.
    RLM_EH_NET_WERR = -104, //Error writing to network.
    RLM_EH_NET_RERR = -105, //Error reading from network.
    RLM_EH_NET_BADRESP = -106,  //Unexpected response.
    RLM_EH_BADHELLO = -107, //HELLO message for wrong server.
    RLM_EH_BADPRIVKEY = -108,   //Error in private key.
    RLM_EH_SIGERROR = -109, //Error signing authorization.
    RLM_EH_INTERNAL = -110, //Internal error.
    RLM_EH_CONN_REFUSED = -111, //Connection refused at server (this can also happen if you have a bad TCP/IP address in your local database).
    RLM_EH_NOSERVER = -112, //No server to connect to.
    RLM_EH_BADHANDSHAKE = -113, //Bad communications handshake.
    RLM_EH_CANTGETETHER = -114, //Can’t get ethernet address.
    RLM_EH_MALLOC = -115,   //malloc() error.
    RLM_EH_BIND = -116, //bind() error.
    RLM_EH_SOCKET = -117,   //socket() error.
    RLM_EH_BADPUBKEY = -118,    //Error in public key.
    RLM_EH_AUTHFAIL = -119, //Authentication failed.
    RLM_EH_WRITE_LF = -120, //Can’t write new license file.
    RLM_EH_DUP_ISV_HID = -122,  //ISV-defined hostid already registered.
    RLM_EH_BADPARAM = -123, //Bad parameter passed to RLM function.
    RLM_EH_ROAMWRITEERR = -124, //Roam File write error.
    RLM_EH_ROAMREADERR = -125,  //Roam File read error.
    RLM_EH_HANDLER_INSTALLED = -126,    //Heartbeat handler already installed.
    RLM_EH_CANTCREATELOCK = -127,   //Can’t create ‘single’ lockfile.
    RLM_EH_CANTOPENLOCK = -128, //Can’t open ‘single’ lockfile.
    RLM_EH_CANTSETLOCK = -129,  //Can’t set lock for ‘single’.
    RLM_EH_BADRLMLIC = -130,    //Bad/missing/expired RLM license.
    RLM_EH_BADHOST = -131,  //Bad hostname in license file or port@host.
    RLM_EH_CANTCONNECTURL = -132,   //Can’t connect to specified URL (activation).
    RLM_EH_OP_NOT_ALLOWED = -133,   //Operation not allowed on server.The status, reread, shutdown, or remove command has been disabled for this user.
    RLM_EH_ACT_BADSTAT = -134,  //Bad status return from Activation server.
    RLM_EH_ACT_BADLICKEY = -135,    //Activation server built with incorrect license key.
    RLM_EH_ACT_BAD_HTTP = -136, //Error in HTTP transaction with Activation server.
    RLM_EH_DEMOEXISTS = -137,   //Demo already created on this system.
    RLM_EH_DEMOWRITEERR = -138, //Demo install file write error.
    RLM_EH_NO_DEMO_LIC = -139,  //No “rlm_demo” license available.
    RLM_EH_NO_RLM_PLATFORM = -140,    //  RLM is unlicensed on this platform.
    RLM_EH_EVAL_EXPIRED = -141, //The RLM evaluation license compiled into this binary has expired.
    RLM_EH_SERVER_REJECT = -142,    //Server rejected (too old).
    RLM_EH_UNLICENSED = -143,   //Unlicensed RLM option.
    RLM_EH_SEMAPHORE_FAILURE = -144,    //Semaphore initialization failure.
    RLM_EH_ACT_OLDSERVER = -145,    //Activation server too old (doesn’t support encryption).
    RLM_EH_BAD_LIC_LINE = -146, //Invalid license line in LF.
    RLM_EH_BAD_SERVER_HOSTID = -147,    //Invalid hostid on SERVER line.
    RLM_EH_NO_REHOST_TOP_DIR = -148,    //No rehostable hostid top-level dir.
    RLM_EH_CANT_GET_REHOST = -149,  //Cannot get rehostable hostid.
    RLM_EH_CANT_DEL_REHOST = -150,  //Cannot delete rehostable hostid.
    RLM_EH_CANT_CREATE_REHOST = -151,   //Cannot create rehostable hostid.
    RLM_EH_REHOST_TOP_DIR_EXISTS = -152,    //Rehostable top directory exists.
    RLM_EH_REHOST_EXISTS = -153,    //Rehostable hostid exists.
    RLM_EH_NO_FULFILLMENTS = -154,  //No fulfillments to revoke.
    RLM_EH_METER_READERR = -155,    //Meter read error.
    RLM_EH_METER_WRITEERR = -156,   //Meter write error.
    RLM_EH_METER_BADINCREMENT = -157,   //Bad meter increment command.
    RLM_EH_METER_NO_COUNTER = -158, //Can’t find counter in meter.
    RLM_EH_ACT_UNLICENSED = -159,   //Activation Unlicensed.
    RLM_EH_ACTPRO_UNLICENSED = -160,    //Activation Pro Unlicensed.
    RLM_EH_SERVER_REQUIRED = -161,  //Counted license requires server.
    RLM_EH_DATE_REQUIRED = -162,    //REPLACE license requires date.
    RLM_EH_NO_METER_UPGRADE = -163, //METERED licenses can’t be UPGRADED.
    RLM_EH_NO_CLIENT = -164,    //Disconnected client data can’t be found.
    RLM_EH_NO_DISCONN = -165,   //Operation not allowed on disconnected handle.
    RLM_EH_NO_FILES = -166, //Too many open files.
    RLM_EH_NO_BROADCAST_RESP = -167,    //No response to broadcast message.
    RLM_EH_NO_BROADCAST_HOST = -168,    //Broadcast response didn’t include hostname.
    RLM_EH_SERVER_TOO_OLD = -169,   //Server too old for disconnected operations.
    RLM_EH_BADLIC_FROM_SERVER = -170,   //License from server doesn’t authenticate.
    RLM_EH_NO_LIC_FROM_SERVER = -171,   //No License returned from server.
    RLM_EH_CACHEWRITEERR = -172,    //Client Cache File write error.
    RLM_EH_CACHEREADERR = -173, //Client Cache File read error.
    RLM_EH_LIC_WITH_NEW_KEYWORDS = -174,    //License returned from server has keywords I don’t understand.
    RLM_EH_NO_ISV = -175,   //Unknown ISV name.
    RLM_EH_NO_CUSTOMER = -176,  //Unknown Customer name.
    RLM_EH_NO_SQL = -177,   //Cannot open MySQL database (RLM Cloud only).
    RLM_EH_ONLY_LOCAL_CMDS = -178,  //Only local command-line commands allowed.
    RLM_EH_SERVER_TIMEOUT = -179,   //Server timeout on read.
    RLM_EH_NONE_SIGNED = -180,  // RLMsign did not sign any licenses (warning).
    RLM_EH_DUP_XFER = -181, //Duplicate disconnected transfer.
    RLM_EH_BADLOGIN = -182, //Bad/No login credentials to server.
    RLM_EH_WS_NOSUPP = -183,    //Function not supported with web services.
    RLM_EH_NOFUNC = -184,   //Function not available.
    RLM_EH_TOOMUCHJSON = -185,  //JSON reply too big.
    RLM_EH_NOLICFROMSERV = -186,    //Server returned no temp license.
    RLM_EH_TEMPFROMCLOUD = -187,    //Temporary licenses come from RLM Cloud servers only.
    RLM_EH_NOTTEMP = -188,  //License is not a temporary license.
    RLM_EH_NOLICENSE = -189,    //No license supplied to call.
    RLM_EH_NOTEMPFROMLOCAL = -190,  //Local license can’t be converted to temp license.
    RLM_EH_NOHTTPSSUPPORT = -191,   //ActPro HTTPS support not available.
    RLM_EH_NOHTTPSDATA = -192,  //No returned data from HTTPS.
    RLM_EH_NOTTHISHOST = -193,  //Wrong Host.
    RLM_EH_NOTRANSBIN = -194,   //Translated binaries not supported (mac).

    // In addition, rlm_activate()/rlm_act_request() will return the following errors:
    RLM_ACT_BADPARAM = -1001,   //Unused – RLM_EH_BADPARAM returned instead.
    RLM_ACT_NO_KEY = -1002, //No activation key supplied.
    RLM_ACT_NO_PROD = -1003,    //No product definition exists.
    RLM_ACT_CANT_WRITE_KEYS = -1004,    //Can’t write keyf table.
    RLM_ACT_KEY_USED = -1005,   //Activation key already used.
    RLM_ACT_BAD_HOSTID = -1006, //Missing hostid.
    RLM_ACT_BAD_HOSTID_TYPE = -1007,    //Invalid hostid type.
    RLM_ACT_BAD_HTTP = -1008,   //Bad HTTP transaction.Note: unused after v3.0BL4.
    RLM_ACT_CANTLOCK = -1009,   //Can’t lock activation database.
    RLM_ACT_CANTREAD_DB = -1010,    //Can’t read activation database.
    RLM_ACT_CANT_WRITE_FULFILL = -1011, //Can’t write licf table.
    RLM_ACT_CLIENT_TIME_BAD = -1012,    //Clock bad on client system (not within 7 days of server).
    RLM_ACT_BAD_REDIRECT = -1013,   //Can’t write licf table.
    RLM_ACT_TOOMANY_HOSTID_CHANGES = -1014, //Too many hostid changes for refresh-type activation.
    RLM_ACT_BLACKLISTED = -1015,    //Domain on blacklist for activation.
    RLM_ACT_NOT_WHITELISTED = -1016,    //Domain not on activation key whitelist.
    RLM_ACT_KEY_EXPIRED = -1017,    //Activation key expired.
    RLM_ACT_NO_PERMISSION = -1018,  //HTTP request denied (this is a setup problem).
    RLM_ACT_SERVER_ERROR = -1019,   //HTTP internal server error(usually a setup problem).
    RLM_ACT_BAD_GENERATOR = -1020,  //Bad/missing generator file (ActPro).
    RLM_ACT_NO_KEY_MATCH = -1021,   //No matching activation key in database.
    RLM_ACT_NO_AUTH_SUPPLIED = -1022,   //No proxy authorization supplied.
    RLM_ACT_PROXY_AUTH_FAILED = -1023,  //Proxy authentication failed.
    RLM_ACT_NO_BASIC_AUTH = -1024,  //No basic authentication supported by proxy.
    RLM_ACT_GEN_UNLICENSED = -1025, //Activation generator unlicensed (ISV_mklic).
    RLM_ACT_DB_READERR = -1026, //Activation database read error (ActPro).
    RLM_ACT_GEN_PARAM_ERR = -1027,  //Generating license - bad parameter.
    RLM_ACT_UNSUPPORTED_CMD = -1028,    //Unsupported command to license generator.
    RLM_ACT_REVOKE_TOOLATE = -1029, //Revoke command after expiration.
    RLM_ACT_KEY_DISABLED = -1030,   //Activation key disabled.
    RLM_ACT_KEY_NO_HOSTID = -1031,  //Key not fulfilled on this hostid.
    RLM_ACT_KEY_HOSTID_REVOKED = -1032, //Key revoked on this hostid.
    RLM_ACT_NO_FULFILLMENTS = -1033,    //No fulfillments to remove.
    RLM_ACT_LICENSE_TOOBIG = -1034, //Generated license too long.
    RLM_ACT_NO_REHOST = -1035,  //Counted licenses can’t be rehostable.
    RLM_ACT_BAD_URL = -1036,    //License Generator not found on server.
    RLM_ACT_NO_LICENSES = -1037,    //Cloud: No licenses found.
    RLM_ACT_NO_CLEAR = -1038,   //Unencrypted requests not allowed.
    RLM_ACT_BAD_KEY = -1039,    //Bad activation key (illegal char).
    RCC_CANT_WRITE_FULFILL = -1040,   	// RLM Cloud: Can’t write licf table.
    RCC_PORTAL_CANT_WRITE_FULFILL = -1041,    // RLM Cloud: Can’t write licf table.
    RLM_ACT_KEY_TOOMANY = -1042,    //Insufficient count left in activation key.
    RLM_ACT_SUB_BADTYPE = -1043,    //Subscription license not Nodelocked or Single.
    RLM_ACT_CONTACT_BAD = -1044,    //Contact information supplied is bad.

    // rlm_license_stat() returns RLM_LICENSE errors and status. These are:
    Success = 0, //Success
    RLM_EL_NOPRODUCT = -1,	//No authorization for product RLM_checkout() did not find a product to satisfy your request.
    RLM_EL_NOTME = -2,  //Authorization is for another ISV,	//The license you are requesting is in the license file, but it is for a different ISV.
    RLM_EL_EXPIRED = -3,    //Authorization has expired,	//The only license available has expired.This error will only be returned for local license lines, never from a license server.
    RLM_EL_NOTTHISHOST = -4,    //Wrong host for authorization. The hostid in the license doesn’t match the hostid of the machine where the software is running.
    RLM_EL_BADKEY = -5, //Bad key in authorization,	//The signature in the license line is not valid, i.e.it does not match the remainder of the data in the license.
    RLM_EL_BADVER = -6, //Requested version not supported,	//Your application tried to check out a license at a higher version than was available, e.g., you specified v5, but the available license is for v4.
    RLM_EL_BADDATE = -7,    //Bad date format - not permanent or dd-mm-yy,	//The expiration, start, or issued date wasn’t understood, eg, 316-mar-2010 or 31-jun-2010. You’d probably never see this in the field unless somebody had tampered with the license file.
    RLM_EL_TOOMANY = -8,    //Checkout request for too many licenses,	//Your checkout request will never work, because you have asked for more licenses than are issued.
    RLM_EL_NOAUTH = -9, //No license auth supplied to call,	//This is an internal error.
    RLM_EL_ON_EXC_ALL = -10,    //On excludeall list,	//The license administrator has specified an EXCLUDEALL list for this product, and the user (host, etc) is on it.
    RLM_EL_ON_EXC = -11,    //On feature exclude list,	//The license administrator has specified an EXCLUDE list for this product, and the user (host, etc) is on it.
    RLM_EL_NOT_INC_ALL = -12,   //Not on the includeall list,	//The license administrator has specified an INCLUDEALL list for this product, and you are not on it.
    RLM_EL_NOT_INC = -13,   //Not on the feature include list,	//The license administrator has specified an INCLUDE list for this product, and you are not on it.
    RLM_EL_OVER_MAX = -14,  //Request would go over license MAX,	//The license administrator set a license MAX usage option for a user or group. This checkout request would put this user/group/host over that limit.
    RLM_EL_REMOVED = -15,   //License (rlm) removed by server,	//A license administrator removed this license using the rlmremove command or the RLM web interface.
    RLM_EL_SERVER_BADRESP = -16,    //Unexpected response from server,	//The application received a response from the license server which it did not expect.This is an internal error.
    RLM_EL_COMM_ERROR = -17,    //Error communicating with server,	//This indicates a basic communication error with the license server, either in a network initialization, read, or write call.
    RLM_EL_NO_SERV_SUPP = -18,  //License server doesn’t support this
    RLM_EL_NOHANDLE = -19,  //No license handle. No license handle supplied to an rlm_get_attr_xxx() call or rlm_license_xxx() call.
    RLM_EL_SERVER_DOWN = -20,   //Server closed connection,	//The license server closed the connection to the application.
    RLM_EL_NO_HEARTBEAT = -21,  //No heartbeat response received,	//Your application did not receive a response to a heartbeat message which it sent. This would happen when you call rlm_get_attr_health(), or automatically if you called rlm_auto_hb().
    RLM_EL_ALLINUSE = -22,  //All licenses in use,	//All licenses are currently in use, and the user did not request to be queued.This request will succeed at some other time when some licenses are checked in.
    RLM_EL_NOHOSTID = -23,  //No hostid on uncounted license. Uncounted licenses always require a hostid.
    RLM_EL_TIMEDOUT = -24,  //License timed out by server,	//Your application did not send any heartbeats to the license server and the license administrator specified a TIMEOUT option in the ISV server options file.
    RLM_EL_INQUEUE = -25,   //In queue for license,	//All licenses are in use, and the user requested queuing by setting the RLM_QUEUE environment variable.
    RLM_EL_SYNTAX = -26,    //License syntax error,	//This is an internal error.
    RLM_EL_ROAM_TOOLONG = -27,  //Roam time exceeds maximum,	//The roam time specified in a checkout request is longer than either the license-specified maximum roaming time or the license administrator’s ROAM_MAX_DAYS option specification.
    RLM_EL_NO_SERV_HANDLE = -28,    //Server does not know this license handle,	//This is an internal server error.It will be returned usually when you are attempting to return a roaming license early.
    RLM_EL_ON_EXC_ROAM = -29,   //On roam exclude list,	//The license administrator has specified an EXCLUDE_ROAM list for this product, and the user (host, etc) is on it.
    RLM_EL_NOT_INC_ROAM = -30,  //Not on the roam include list,	//The license administrator has specified an INCLUDE_ROAM list for this product, and you are not on it.
    RLM_EL_TOOMANY_ROAMING = -31,   //Too many licenses roaming already,	//A request was made to roam a license, but there are too many licenses roaming already (set by the license administrator ROAM_MAX_COUNT option).
    RLM_EL_WILL_EXPIRE = -32,   //License expires before roam period ends,	//A roaming license was requested, but the only license which can fulfill the request will expire before the roam period ends.
    RLM_EL_ROAMFILEERR = -33,   //Problem with roam file,	//There was a problem writing the roam data file on the application’s computer.
    RLM_EL_RLM_ROAM_ERR = -34,  //Cannot check out rlm_roam license,	//A license was requested to roam, but the application cannot check out an rlm_roam license.
    RLM_EL_WRONG_PLATFORM = -35,    //Wrong platform for client,	//The license specifies platforms= xxx, but the application is not running on one of these platforms.
    RLM_EL_WRONG_TZ = -36,  //Wrong timezone for client. The license specifies an allowed timezone, but the application is running on a computer in a different timezone.
    RLM_EL_NOT_STARTED = -37,   //License start date in the future,	//The start date in the license hasn’t occurred yet, e.g., today you try to check out a license containing start = 1 - mar - 2030.
    RLM_EL_CANT_GET_DATE = -38,   //time() call failure. The time() system call failed.
    RLM_EL_OVERSOFT = -39,  //Request goes over. This license checkout causes the license usage to license soft_limit go over it’s soft limit.The checkout is successful, but usage is now in the overdraft mode.RLM_EL_OVERSOFT is also returned if you have a misconfigured token-based license and the server has gone into overdraft due to this. See the note in the token-based license restrictions section.
    RLM_EL_WINDBACK = -40,  //Clock setback detected. RLM has detected that the clock has been set back.This error will only happen on expiring licenses.
    RLM_EL_BADPARAM = -41,  //Bad parameter to rlm_checkout() call,	//This currently happens if a checkout request is made for < 0 licenses.
    RLM_EL_NOROAM_FAILOVER = -42,   //Roam operations not allowed on failover server. A failover server has taken over for a primary server, and a roaming license was requested. Roaming licenses can only be obtained from primary servers.Re-try the request later when the primary server is up.
    RLM_EL_BADHOST = -43,   //bad hostname in license file or port@host. The hostname in the license file is not valid on this network.
    RLM_EL_APP_INACTIVE = -44,  //Application is inactive,	//Your application is set to the inactive state (with rlm_set_active(rh, 0), and you have called rlm_get_attr_health().
    RLM_EL_NOT_NAMED_USER = -45,    //User is not on the named-user list,	//You are not on the named user list for this product.
    RLM_EL_TS_DISABLED = -46,   //Terminal server/remote desktop disabled,	//The only available license has Terminal Server disabled, and the application is running on a Windows Terminal Server machine.
    RLM_EL_VM_DISABLED = -47,   //Running on Virtual Machines disabled,	//The only available license has virtual machines disabled, and the application is running on a virtual machine.
    RLM_EL_PORTABLE_REMOVED = -48,  //Portable hostid removed,	//The license is locked to a portable hostid (dongle), and the hostid was removed after the license was acquired by the application.
    RLM_EL_DEMOEXP = -49,   //Demo license has expired,	//Detached Demotm license has expired.
    RLM_EL_FAILED_BACK_UP = -50,    //Failed host back up - failover server released license,	//If your application is holding a license from a failover server, when the main server comes back up, the failover server will drop all the licenses it is serving, and you will get this status.
    RLM_EL_SERVER_LOST_XFER = -51,  //Server lost it’s transferred license,	//Your license was served by a server which had received transferred licenses from another license server.The originating license server may have gone down, in which case, your server will lose the licenses which were transferred to it.
    RLM_EL_BAD_PASSWORD = -52,  //Incorrect password for product. RLM_EL_BAD_PASSWORD is an internal error and won’t ever be returned to the client - if the license password is bad, the client will receive RLM_EL_NO_SERV_SUPP.
    RLM_EL_METER_NO_SERVER = -53,	//Metered licenses,	//Metered licenses only work with with a license require server server.
    RLM_EL_METER_NOCOUNT = -54,	//Not enough count for meter,	//There is insufficient count in the meter for the requested operation.
    RLM_EL_NOROAM_TRANSIENT = -55,	//Roaming not allowed,	//Roaming is not allowed on servers with transient hostids, ie, dongles.
    RLM_EL_CANTRECONNECT = -56,	//Can’t reconnect to server,	//On a disconnected handle, the operation requested needed to reconnect to the server, and this operation failed.
    RLM_EL_NONE_CANROAM = -57,	//None of these licenses can roam,	//The license max_roam_count is set to 0. This will always be the case for licenses that are transferred to another server.
    RLM_EL_SERVER_TOO_OLD = -58,	//Server too old for this operation,	//In v10, this error means that disconnected operation (rlm_init_disconn()) was attempted on a pre-v10.0 license server.
    RLM_EH_SERVER_REJECT_ = -59,	//Server rejected client,	//Either the server is older than the oldest version allowed, or a generic server is used when the client specifies this is not allowed.
    RLM_EL_REQ_OPT_MISSING = -60,	//Required option missing,	//A required option was set with the rlm_set_req_opt() call, and this string is not part of the license string.This error will only be reported for nodelocked licenses, the server will always report RLM_EL_NO_SERV_SUPP.
    RLM_EL_NO_DYNRES = -61,	//Reclaim can’t find dynamic reservation,	//The license specified to be reclaimed cannot be found.
    RLM_EL_RECONN_INFO_BAD = -62,	//Reconnection info invalid,	//This is generally an internal error.
    RLM_EL_ALREADY_ROAMING = -63,	//License already roaming on this host,	//If you attempt to roam N licenses then later N+X licenses, you will receive this error.The original roam must be returned first.
    RLM_EL_BAD_EXTEND_FMT = -64,	//Bad format for RLM_ROAM_EXTEND.RLM_ROAM_EXTEND format is product:date:extension_code.
    RLM_EL_BAD_EXTEND_CODE = -65,	//Bad extend code,	//Extension code does not verify correctly.
    RLM_EL_NO_ROAM_TO_EXTEND = -66,	//No roaming license to extend,	//This is an attempt to extend a non-existant roaming license.
    RLM_EL_NESTED_ALIAS = -67,	//Nested aliases,	//You cannot define an alias in terms of another alias.
    RLM_EL_NO_JSON = -68,	//No JSON in returned message,	//This is an internal error in web services processing with RLM Cloud.
    RLM_EL_BAD_JSON = -69,	//Bad JSON in returned message,	//This is an internal error in web services processing with RLM Cloud.
    RLM_EL_BADHANDSHAKE = -70,	//Bad handshake on web services checkout,	//This is an internal error in web services processing with RLM Cloud.
    RLM_EL_HELPER_ERR = -71,   //RLM_helper error,	//This is an internal error in web services processing with RLM Cloud.
    RLM_EL_PERS_NOT_ON_LIST = -72,	//Not on list,	//The user is not on the personal license list.
    RLM_EL_PERS_BADPASS = -73,	//Bad password,	//The personal user’s password is incorrect.
    RLM_EL_PERS_INUSE = -74,	//License in use,	//The personal license is in use.
    RLM_EL_PERS_HANDLE_ERR = -75,	//Handle error.RLM handle error(personal license)
    RLM_EL_NOTTEMP = -76,	//Not a temp license,	//License is not a temporary license.
    RLM_EL_NOSERVER = -77,	//No server,	//No server to connect to return temp license.
}
