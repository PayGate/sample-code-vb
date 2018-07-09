@*
    Sample page showing data to send to PayGate PayWeb3 to query transaction results
    and
    the results from the query
*@

@Imports System.Text
@Code
    'Data to query PayWeb 3
    Dim data = New Dictionary(Of String, String)() From {
{"PAYGATE_ID", Request("PAYGATE_ID")},
{"PAY_REQUEST_ID", Request("PAY_REQUEST_ID")},
{"REFERENCE", Request("REFERENCE")}
}

    'Set encryption key, default if absent
    Dim encryption_key = If(Request("encryption_key") <> "", Request("encryption_key"), "secret")

    Dim pgid = If(data("PAYGATE_ID") <> "", data("PAYGATE_ID"), "10011072130")
    Dim payrequestid = If(data("PAY_REQUEST_ID") <> "", data("PAY_REQUEST_ID"), "")
    Dim reference = If(data("REFERENCE") <> "", data("REFERENCE"), "")

    Dim pg3 = New PaygatePayweb3()
    pg3.setEncryptionKey(encryption_key)

    'Set request data
    pg3.setQueryRequest(data)

    'Execute query
    Dim returnData = pg3.doQuery()

    Dim htmlError = New StringBuilder("<div class=""well"">")

    If pg3.queryResponse.Count > 0 Then
        'We have received a reponse from PayWeb 3
        If pg3.lastError Is Nothing Then
            'No error
            For Each item In pg3.queryResponse
                htmlError.Append("<div class=""row"">")
                htmlError.Append("<label for=""" & item.Key & "_RESPONSE"" class=""col-sm-3"">" + item.Key & "</label>")
                htmlError.Append("<div class=""col-sm-9""><p id=""" & item.Key & "_RESPONSE"">" + item.Value & "</p></div></div>")
            Next
        Else
            htmlError.Append(pg3.lastError)
        End If
    End If

    htmlError.Append("</div>")
    Dim htmlErrorRaw = Html.Raw(htmlError)
End Code

<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayWeb 3 - Query</title>
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
                        <li>
                            <a href="index.vbhtml">Initiate</a>
                        </li>
                        <li class="active">
                            <a href="query.vbhtml">Query</a>
                        </li>
                        <li>
                            <a href="simple_query.vbhtml">Simple query</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
        <div style="background-color:#80b946; text-align: center; margin-top: 51px; margin-bottom: 15px; padding: 4px;"><strong>Query</strong></div>
        <div class="container">
            <form role="form" class="form-horizontal text-left" action="query.vbhtml" method="post">
                <div class="form-group">
                    <label for="PAYGATE_ID" class="col-sm-3 control-label">PayGate ID</label>
                    <div class="col-sm-6">
                        <input type="text" name="PAYGATE_ID" id="PAYGATE_ID" class="form-control" value="@pgid" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="PAY_REQUEST_ID" class="col-sm-3 control-label">Pay Request ID</label>
                    <div class="col-sm-6">
                        <input type="text" name="PAY_REQUEST_ID" id="PAY_REQUEST_ID" class="form-control" value="@payrequestid" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="REFERENCE" class="col-sm-3 control-label">Reference</label>
                    <div class="col-sm-6">
                        <input type="text" name="REFERENCE" id="REFERENCE" class="form-control" value="@reference" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="encryption_key" class="col-sm-3 control-label">Encryption Key</label>
                    <div class="col-sm-6">
                        <input type="text" name="encryption_key" id="encryption_key" class="form-control" value="@encryption_key" />
                    </div>
                </div>
                <br>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-2">
                        <input type="submit" id="doQueryBtn" class="btn btn-success btn-block" value="Do Query" name="btnSubmit">
                    </div>
                    <div class="col-sm-2">
                        <a href="index.vbhtml" class="btn btn-primary btn-block">New Transaction</a>
                    </div>
                </div>
            </form>
            <br>
            @htmlErrorRaw
        </div>
    </div>

    </div>

    <script type="text/javascript" src="../lib/js/jquery-1.10.2.min.js"></script>

    <script type="text/javascript" src="../lib/js/bootstrap.min.js"></script>

</body>

</html>

