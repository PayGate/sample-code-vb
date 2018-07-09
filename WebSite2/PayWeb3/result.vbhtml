﻿@*
    Once the client has completed the transaction on the PayWeb page, they will be redirected to the RETURN_URL set in the initate
    Here we will check the transaction status and process accordingly
*@

@Imports System.Text.RegularExpressions
@Imports System.Text
@Code
    'Insert the returned data as well as the merchant specific data PAYGATE_ID And REFERENCE
    Dim data = New Dictionary(Of String, String)() From {
{"PAYGATE_ID", If(Regex.IsMatch(SessionModel.pgid, "^\d{9,12}$"), SessionModel.pgid, "INVALID ID")},
{"PAY_REQUEST_ID", If(Regex.IsMatch(Request("PAY_REQUEST_ID"), "^[A-Z0-9]{8}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12}$"), Request("PAY_REQUEST_ID"), "INVALID PAY_REQUEST_ID")},
{"TRANSACTION_STATUS", If(Regex.IsMatch(Request("TRANSACTION_STATUS"), "^[A-Za-z0-9]*$"), Request("TRANSACTION_STATUS"), "INVALID TRANSACTION STATUS")},
{"REFERENCE", SessionModel.reference},
{"CHECKSUM", If(Regex.IsMatch(Request("CHECKSUM"), "^[A-Za-z0-9]*$"), Request("CHECKSUM"), "INVALID CHECKSUM")}
}

    Dim checksum = data("CHECKSUM")

    Dim pg3 = New PaygatePayweb3
    pg3.setEncryptionKey(SessionModel.key)

    'Validate returned checksum
    Dim isValid = pg3.validateChecksum(data)

    Dim validHtml = New StringBuilder()
    validHtml.Append(If(Not isValid, "The checksums do not match <i class=""glyphicon glyphicon-remove text-danger""></i>", "Checksums match OK <i class=""glyphicon glyphicon - ok text - success""></i>"))
    Dim validText = Html.Raw(validHtml)
    Dim transactionStatus As Integer
    Int32.TryParse(data("TRANSACTION_STATUS"), transactionStatus)
    Dim key = SessionModel.key
End Code

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayWeb 3 - Result</title>
    <link rel="stylesheet" href="../lib/css/bootstrap.min.css">
    <link rel="stylesheet" href="../lib/css/core.css">
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
                        <img alt="PayGate" src="../lib/images/paygate_logo_mini.png" />
                    </a>
                    <span style="color: #f4f4f4; font-size: 18px; line-height: 45px; margin-right: 10px;"><strong>PayWeb 3</strong></span>
                </div>
                <div class="collapse navbar-collapse" id="navbar-collapse">
                    <ul class="nav navbar-nav">
                        <li class="active">
                            <a href="index.vbhtml">Initiate</a>
                        </li>
                        <li>
                            <a href="query.vbhtml">Query</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
        <div style="background-color:#80b946; text-align: center; margin-top: 51px; margin-bottom: 15px; padding: 4px;"><strong>Result</strong></div>
        <div class="container">
            <form role="form" class="form-horizontal text-left" action="query.vbhtml" method="post" name="query_paygate_form">
                <div class="form-group">
                    <label for="checksumResult" class="col-sm-3 control-label">Checksum result</label>
                    <p id="checksumResult" class="form-control-static">@validText</p>
                </div>
                <hr>
                <div class="form-group">
                    <label for="PAY_REQUEST_ID" class="col-sm-3 control-label">Pay Request ID</label>
                    <p id="PAY_REQUEST_ID" class="form-control-static">@data("PAY_REQUEST_ID")</p>
                </div>
                <div Class="form-group">
                    <Label for="TRANSACTION_STATUS" class="col-sm-3 control-label">Transaction Status</Label>
                    <p id="TRANSACTION_STATUS" Class="form-control-static">@data("TRANSACTION_STATUS") - @pg3.getTransactionStatusDescription(transactionStatus)</p>
                </div>
                <div class="form-group">
                    <label for="CHECKSUM" class="col-sm-3 control-label">Checksum</label>
                    <p id="CHECKSUM" class="form-control-static">@checksum</p>
                </div>

                <!-- Hidden fields to post to the Query service -->
                <input type="hidden" name="PAYGATE_ID" value="@data("PAYGATE_ID")" />
                <input type="hidden" name="PAY_REQUEST_ID" value="@data("PAY_REQUEST_ID")" />
                <input type="hidden" name="REFERENCE" value="@data("REFERENCE")" />
                <input type="hidden" name="encryption_key" value="@key" />
                <!-- -->

                <div class="form-group">
                    <div class="col-sm-2">
                        <input type="submit" class="btn btn-success btn-block" value="Query PayGate" name="btnSubmit">
                    </div>
                    <div class="col-sm-offset-2 col-sm-2">
                        <input type="submit" formaction="simple_query.vbhtml" class="btn btn-success btn-block" value="Simple Query">
                    </div>
                    <div class="col-sm-offset-2 col-sm-2">
                        <a href="index.vbhtml" class="btn btn-primary btn-block">New Transaction</a>
                    </div>
                </div>
            </form>
        </div>
    </div>
    <script type="text/javascript" src="../lib/js/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="../lib/js/bootstrap.min.js"></script>
</body>
</html>