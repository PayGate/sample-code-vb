@*
    File showing inputs required for a Vault Payment
*@

@Code
    Dim pgid = If(Request("payGateId") IsNot Nothing, Request("payGateId"), PayhostSOAP.DEFAULT_PGID)
    Dim encryptionKey = If(Request("encryptionKey") IsNot Nothing, Request("encryptionKey"), PayhostSOAP.DEFAULT_ENCRYPTION_KEY)
    Dim cardNumber = If(Request("cardNumber") IsNot Nothing, Request("cardNumber"), "")
    Dim expiryDate = If(Request("expiryDate") IsNot Nothing, Request("expiryDate"), "")

    Dim queryResponse = ""
    Dim queryRequest = ""

    If Request("btnSubmit") IsNot Nothing Then
        'Create the PayHost client
        Dim payHostt As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Dim cardVaultRequest As PayHost.CardVaultRequestType = PayhostSOAP.makeCardVaultRequest(Request)

        queryRequest = PayhostSOAP.getXMLText(cardVaultRequest)
        Dim sprequest = New PayHost.SingleVaultRequest With {
            .Item = cardVaultRequest
        }
        Dim sprequest1 = New PayHost.SingleVaultRequest1(sprequest)

        Try
            Dim response = payHostt.SingleVault(sprequest1)
            Dim r = TryCast(response.SingleVaultResponse.Item, PayHost.CardVaultResponseType)
            queryResponse = PayhostSOAP.getXMLText(r)
        Catch e As Exception
            Dim err = e.Message
            queryResponse = err
        End Try
    End If
End Code

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayHost - Card Vault</title>
    <link rel="stylesheet" href="../../../lib/css/bootstrap.min.css">
    <link rel="stylesheet" href="../../../lib/css/core.css">
</head>
<body>
    <div class="container-fluid" style="min-width: 320px;">
        <nav class="navbar navbar-inverse navbar-fixed-top">
            <div class="container-fluid">
                <!-- Brand and toggle get grouped for better mobile display -->
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#navbar-collapse">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <a class="navbar-brand" href="">
                        <img alt="PayGate" src="../../../lib/images/paygate_logo_mini.png" />
                    </a>
                    <span style="color: #f4f4f4; font-size: 18px; line-height: 45px; margin-right: 10px;"><strong>PayHost Card Vault</strong></span>
                </div>
                <div class="collapse navbar-collapse" id="navbar-collapse">
                    <ul class="nav navbar-nav">
                        <li class="dropdown active">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">Card Vault</a>
                            <ul class="dropdown-menu" role="menu">
                                <li>
                                    <a href="index.vbhtml">Card Vault</a>
                                </li>
                                <li>
                                    <a href="simple_cardVault.vbhtml">Simple Version</a>
                                </li>
                            </ul>
                        </li>
                        <li class="dropdown">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">Delete Vault</a>
                            <ul class="dropdown-menu" role="menu">
                                <li>
                                    <a href="../deleteVault/index.vbhtml">Delete Vault</a>
                                </li>
                                <li>
                                    <a href="../deleteVault/simple_deleteVault.vbhtml">Simple Version</a>
                                </li>
                            </ul>
                        </li>
                        <li class="dropdown">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">Lookup Vault</a>
                            <ul class="dropdown-menu" role="menu">
                                <li>
                                    <a href="../../../PayHost/singleVault/lookupVault/index.vbhtml">Lookup Vault</a>
                                </li>
                                <li>
                                    <a href="../../../PayHost/singleVault/lookupVault/simple_lookupVault.vbhtml">Simple Version</a>
                                </li>
                            </ul>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
        <div style="background-color:#80b946; text-align: center; margin-top: 51px; margin-bottom: 15px; padding: 4px;"><strong>Card Vault</strong></div>
        <div class="container">
            <form role="form" class="form-horizontal text-left" action="index.vbhtml" method="post">
                <div class="form-group">
                    <label for="payGateId" class="col-sm-3 control-label">PayGate ID</label>
                    <div class="col-sm-5">
                        <input class="form-control" type="text" name="payGateId" id="payGateId" value="@pgid" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="encryptionKey" class="col-sm-3 control-label">Encryption Key</label>
                    <div class="col-sm-5">
                        <input class="form-control" type="text" name="encryptionKey" id="encryptionKey" value="@encryptionKey" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="cardNumber" class="col-sm-3 control-label">Card Number</label>
                    <div class="col-sm-5">
                        <input class="form-control" type="text" name="cardNumber" id="cardNumber" value="@cardNumber" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="expiryDate" class="col-sm-3 control-label">Card Expiry Date</label>
                    <div class="col-sm-5">
                        <input class="form-control" type="text" name="expiryDate" id="expiryDate" value="@expiryDate" placeholder="mmYYYY" />
                    </div>
                </div>
                <div class="form-group userDefined">
                    <label for="userFields" class="col-sm-3 control-label">User Defined</label>
                    <div class="col-sm-4">
                        <input class="form-control userKey" type="text" name="userKey1" id="userKey1" value="" placeholder="Key" />
                    </div>
                    <div class="col-sm-4">
                        <input class="form-control userField" type="text" name="userField1" id="userField1" value="" placeholder="Value" />
                    </div>
                </div>

                <span id="fieldHolder"></span>
                <div class="form-group">
                    <div class="col-sm-offset-3 col-sm-4">
                        <button class="btn btn-primary" id="addUserFieldBtn" type="button"><i class="glyphicon glyphicon-plus"></i> Add User Defined Fields</button>
                    </div>
                </div>
                <br>
                <div class="form-group">
                    <div class=" col-sm-offset-4 col-sm-4">
                        <img src="../../../lib/images/loader.gif" alt="Processing" class="initialHide" id="vaultLoader">
                        <input class="btn btn-success btn-block" id="doVaultBtn" type="submit" name="btnSubmit" value="Do Card Vault" />
                    </div>
                </div>
                <br>
            </form>
            <div class="row" style="margin-bottom: 15px;">
                <div class="col-sm-offset-4 col-sm-4">
                    <button id="showRequestBtn" class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#requestDiv" aria-expanded="false" aria-controls="requestDiv">
                        Request
                    </button>
                </div>
            </div>
            <div id="requestDiv" class="row collapse well well-sm">
                <textarea rows="20" cols="100" id="request" class="form-control">@queryRequest</textarea>
            </div>
            <div class="row" style="margin-bottom: 15px;">
                <div class="col-sm-offset-4 col-sm-4">
                    <button id="showResponseBtn" class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#responseDiv" aria-expanded="false" aria-controls="responseDiv">
                        Response
                    </button>
                </div>
            </div>
            <div id="responseDiv" class="row collapse well well-sm">
                <textarea rows="20" cols="100" id="response" class="form-control">@queryResponse</textarea>
            </div>
        </div>
    </div>
    <script type="text/javascript" src="../../../lib/js/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="../../../lib/js/bootstrap.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#doAuthBtn').on('click', function () {
                $(this).hide();
                $('#authLoader').show();
            });

            $('#doSubmitBtn').on('click', function () {
                $(this).hide();
                $('#submitLoader').show();
            });

            $('#addUserFieldBtn').on('click', function () {

                var lastUserFieldDiv = $('#fieldHolder').prev('.userDefined');

                var key = lastUserFieldDiv.find('.userKey');

                var i = parseInt(key.attr('id').replace('userKey', ''));
                i++;

                var newUserFieldsDiv = '<div class="form-group userDefined">' +
                    '    <label for="reference" class="col-sm-3 control-label">User Defined</label>' +
                    '    <div class="col-sm-4">' +
                    '        <input class="form-control userKey" type="text" name="userKey' + i + '" id="userKey' + i + '" value="" placeholder="Key" />' +
                    '    </div>' +
                    '    <div class="col-sm-4">' +
                    '        <input class="form-control userField" type="text" name="userField' + i + '" id="userField' + i + '" value="" placeholder="Value" />' +
                    '    </div>' +
                    '</div>';

                $('#fieldHolder').before(newUserFieldsDiv);
            });
        });
    </script>
</body>
</html>
