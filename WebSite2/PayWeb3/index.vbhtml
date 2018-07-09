@*
    This is a sample page of the form fields required for a PayGate PayWeb 3 transaction
*@

@Code
    Dim reference = GlobalUtility.generateReference()
    Dim scheme = GlobalUtility.getScheme()
    Dim host = GlobalUtility.getHost()
    Dim countrySelection = Html.Raw(GlobalUtility.generateCountrySelectOptions())
End Code

<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayWeb 3 - Initiate</title>
    <link rel="stylesheet" href="../lib/css/bootstrap.min.css">
    <link rel="stylesheet" href="../lib/css/core.css">
</head>
<body>
    <div Class="container-fluid" style="min-width: 320px;">
        <nav Class="navbar navbar-inverse navbar-fixed-top">
            <div Class="container-fluid">
                <!-- Brand And toggle get grouped for better mobile display -->
                <div Class="navbar-header">
                    <Button type="button" Class="navbar-toggle collapsed" data-toggle="collapse" data-target="#navbar-collapse">
                        <span Class="sr-only">Toggle navigation</span>
                        <span Class="icon-bar"></span>
                        <span Class="icon-bar"></span>
                        <span Class="icon-bar"></span>
                    </Button>
                    <a Class="navbar-brand" href="">
                        <img alt="PayGate" src="../lib/images/paygate_logo_mini.png" />
                    </a>
                    <span style="color: #f4f4f4; font-size: 18px; line-height: 45px; margin-right: 10px;"> <strong> PayWeb 3</strong></span>
                </div>
                <div Class="collapse navbar-collapse" id="navbar-collapse">
                    <ul Class="nav navbar-nav">
                        <li Class="active">
                            <a href="index.vbhtml"> Initiate</a>
                        </li>
                        <li>
                            <a href="query.vbhtml"> Query</a>
                        </li>
                        <li>
                            <a href="simple_initiate.vbhtml"> Simple initiate</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
        <div style="background-color:#80b946; text-align: center; margin-top: 51px; margin-bottom: 15px; padding: 4px;" <> strong >Step 1: Initiate</strong></div>
        <div Class="container">
            <form role="form" Class="form-horizontal text-left" action="request.vbhtml" method="post" name="paygate_initiate_form">
                <div Class="form-group">
                    <Label for="PAYGATE_ID" class="col-sm-3 control-label">PayGate ID</Label>
                    <div Class="col-sm-6">
                        <input type="text" name="PAYGATE_ID" id="PAYGATE_ID" Class="form-control" value="10011072130" />
                    </div>
                </div>
                <div Class="form-group">
                    <Label for="REFERENCE" class="col-sm-3 control-label">Reference</Label>
                    <div Class="col-sm-6">
                        <input type="text" name="REFERENCE" id="REFERENCE" Class="form-control" value="@reference" />
                    </div>
                </div>
                <div Class="form-group">
                    <Label for="AMOUNT" class="col-sm-3 control-label">Amount</Label>
                    <div Class="col-sm-6">
                        <input type="text" name="AMOUNT" id="AMOUNT" Class="form-control" value="100" />
                    </div>
                </div>
                <div Class="form-group">
                    <Label for="CURRENCY" class="col-sm-3 control-label">Currency</Label>
                    <div Class="col-sm-6">
                        <input type="text" name="CURRENCY" id="CURRENCY" Class="form-control" value="ZAR" />
                    </div>
                </div>
                <div Class="form-group">
                    <Label for="RETURN_URL" class="col-sm-3 control-label">Return URL</Label>
                    <div Class="col-sm-6">
                        <input type="text" name="RETURN_URL" id="RETURN_URL" Class="form-control" value="@scheme://@host/PayWeb3/result.vbhtml" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="TRANSACTION_DATE" class="col-sm-3 control-label">Transaction Date</label>
                    <div class="col-sm-6">
                        <input type="text" name="TRANSACTION_DATE" id="TRANSACTION_DATE" class="form-control" value="@DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="LOCALE" class="col-sm-3 control-label">Locale</label>
                    <div class="col-sm-6">
                        <input type="text" name="LOCALE" id="LOCALE" class="form-control" value="en-za" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="COUNTRY" class="col-sm-3 control-label">Country</label>
                    <div class="col-sm-6">
                        <select name="COUNTRY" id="COUNTRY" class="form-control">
                            @countrySelection
                        </select>
                    </div>
                </div>
                <div class="form-group">
                    <label for="EMAIL" class="col-sm-3 control-label">Customer Email</label>
                    <div class="col-sm-6">
                        <input type="text" name="EMAIL" id="EMAIL" class="form-control" value="support@paygate.co.za" />
                    </div>
                </div>
                <br>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <button type="button" class="btn btn-primary btn-block" data-toggle="collapse" data-target="#extraFieldsDiv" aria-expanded="false" aria-controls="extraFieldsDiv">
                            Extra Fields
                        </button>
                    </div>
                </div>
                <div id="extraFieldsDiv" class="collapse well well-sm">
                    <div class="form-group">
                        <label for="PAY_METHOD" class="col-sm-3 control-label">Pay Method</label>
                        <div class="col-sm-6">
                            <input type="text" name="PAY_METHOD" id="PAY_METHOD" class="form-control" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="PAY_METHOD_DETAIL" class="col-sm-3 control-label">Pay Method Detail</label>
                        <div class="col-sm-6">
                            <input type="text" name="PAY_METHOD_DETAIL" id="PAY_METHOD_DETAIL" class="form-control" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="NOTIFY_URL" class="col-sm-3 control-label">Notify URL</label>
                        <div class="col-sm-6">
                            <input type="text" name="NOTIFY_URL" id="NOTIFY_URL" class="form-control" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="USER1" class="col-sm-3 control-label">User Field 1</label>
                        <div class="col-sm-6">
                            <input type="text" name="USER1" id="USER1" class="form-control" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="USER2" class="col-sm-3 control-label">User Field 2</label>
                        <div class="col-sm-6">
                            <input type="text" name="USER2" id="USER2" class="form-control" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="USER3" class="col-sm-3 control-label">User Field 3</label>
                        <div class="col-sm-6">
                            <input type="text" name="USER3" id="USER3" class="form-control" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="VAULT" class="col-sm-3 control-label">Vault</label>
                        <div class="col-sm-6">
                            <div class="radio">
                                <label>
                                    <input type="radio" name="VAULT" id="VAULTOFF" value="" checked>
                                    No card Vaulting
                                </label>
                            </div>
                            <div class="radio">
                                <label>
                                    <input type="radio" name="VAULT" id="VAULTNO" value="0">
                                    Don't Vault card
                                </label>
                            </div>
                            <div class="radio">
                                <label>
                                    <input type="radio" name="VAULT" id="VAULTYES" value="1">
                                    Vault card
                                </label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="VAULT_ID" class="col-sm-3 control-label">Vault ID</label>
                        <div class="col-sm-6">
                            <input type="text" name="VAULT_ID" id="VAULT_ID" class="form-control" placeholder="optional" />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <label for="encryption_key" class="col-sm-3 control-label">Encryption Key</label>
                    <div class="col-sm-6">
                        <input type="text" name="encryption_key" id="encryption_key" class="form-control" value="secret" />
                    </div>
                </div>
                <br>
                <div class="form-group">
                    <div class=" col-sm-offset-4 col-sm-4">
                        <input type="submit" name="btnSubmit" class="btn btn-success btn-block" value="Calculate Checksum" />
                    </div>
                </div>
                <br>
            </form>
        </div>
    </div>
    <script type="text/javascript" src="../lib/js/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="../lib/js/bootstrap.min.js"></script>
</body>
</html>