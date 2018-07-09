@*
    Once the client is ready to be redirected to the payment page, we get all the information needed and initiate the transaction with PayGate.
    This checks that all the information is valid and that a transaction can take place.
    If the initiate is successful we are returned a request ID and a checksum which we will use to redirect the client to PayWeb3.
*@

@Imports System.Text.RegularExpressions
@Imports System.Text
@Code
    Dim processUrl = PaygatePayweb3.process_url
    Dim optionalHtml = Html.Raw("")

    'Get the mandatory request data from the request And do some validation checks on it
    'Test for "Reference" Is only for these examples
    Dim mandatoryFields = New Dictionary(Of String, String)() From {
{"PAYGATE_ID", If(Regex.IsMatch(Request("PAYGATE_ID"), "^\d{9,12}$"), Request("PAYGATE_ID"), "INVALID ID")},
{"REFERENCE", If(Regex.IsMatch(Request("REFERENCE"), "^pgtest_\d{14}$"), Request("REFERENCE"), "INVALID REFERENCE")},
{"AMOUNT", If(Regex.IsMatch(Request("AMOUNT"), "^\d*\.?\d{0,2}$"), Request("AMOUNT"), "INVALID AMOUNT")},
{"CURRENCY", If(Regex.IsMatch(Request("CURRENCY"), "^[A-Z]{3}$"), Request("CURRENCY"), "INVALID CURRENCY")},
{"RETURN_URL", If(Regex.IsMatch(Request("RETURN_URL"), "^(https?:\/\/)?([a-z0-9\.-]+):?\.?([/\w\.-]*)*\/?$"), Request("RETURN_URL"), "INVALID RETURN_URL")},
{"TRANSACTION_DATE", If(Regex.IsMatch(Request("TRANSACTION_DATE"), "^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$"), Request("TRANSACTION_DATE"), "INVALID TRANSACTION DATE")},
{"LOCALE", If(Regex.IsMatch(Request("LOCALE"), "^[a-z]{2}-[a-zA-Z]{2}$"), Request("LOCALE"), "INVALID LOCALE")},
{"COUNTRY", If(Regex.IsMatch(Request("COUNTRY"), "^[A-Z]{3}$"), Request("COUNTRY"), "INVALID COUNTRY")},
{"EMAIL", If(Regex.IsMatch(Request("EMAIL"), "^([a-z0-9_\.-]+)@([a-z0-9_\.-]+)\.([a-z\.]{2,6})$"), Request("EMAIL"), "INVALID EMAIL")}
}

    'Set the session variables needed on the result page
    SessionModel.pgid = mandatoryFields("PAYGATE_ID")
    SessionModel.reference = mandatoryFields("REFERENCE")
    SessionModel.key = Request("encryption_key")

    'Get the optional fields And validate
    Dim optionalFields = New Dictionary(Of String, String)() From {
{"PAY_METHOD", If(Request("PAY_METHOD") <> "", If(Regex.IsMatch(Request("PAY_METHOD"), "^[a-zA-Z ]{0,10}$"), Request("PAY_METHOD"), "INVALID PAY METHOD"), "")},
{"PAY_METHOD_DETAIL", If(Request("PAY_METHOD_DETAIL") <> "", If(Regex.IsMatch(Request("PAY_METHOD_DETAIL"), "^[a-zA-Z ]{0,10}$"), Request("PAY_METHOD_DETAIL"), "INVALID PAY METHOD"), "")},
{"NOTIFY_URL", If(Request("NOTIFY_URL") <> "", If(Regex.IsMatch(Request("NOTIFY_URL"), "^(https?:\/\/)?([a-z0-9\.-]+):?\.?([/\w\.-]*)*\/?$"), Request("NOTIFY_URL"), "INVALID URL"), "")},
{"USER1", If(Request("USER1") <> "", If(Regex.IsMatch(Request("USER1"), "^[a-zA-Z \d]{0,10}$"), Request("USER1"), "INVALID FORMAT"), "")},
{"USER2", If(Request("USER2") <> "", If(Regex.IsMatch(Request("USER2"), "^[a-zA-Z \d]{0,10}$"), Request("USER2"), "INVALID FORMAT"), "")},
{"USER3", If(Request("USER3") <> "", If(Regex.IsMatch(Request("USER3"), "^[a-zA-Z \d]{0,10}$"), Request("USER3"), "INVALID FORMAT"), "")},
{"VAULT", If(Request.Form.AllKeys.Contains("VAULT"), If(Regex.IsMatch(Request("VAULT"), "^[a-zA-Z \d]{0,10}$"), Request("VAULT"), "INVALID FORMAT"), "")},
{"VAULT_ID", If(Request.Form.AllKeys.Contains("VAULT_ID"), If(Regex.IsMatch(Request("VAULT_ID"), "^[a-zA-Z \d]{0,10}$"), Request("VAULT_ID"), "INVALID FORMAT"), "")}
}

    'Merge the optional fields
    For Each item In optionalFields
        mandatoryFields.Add(item.Key, item.Value)
    Next

    Dim data = mandatoryFields
    Dim displayOptionalFields = False

    For Each item In optionalFields

        If data(item.Key) <> "" Then
            displayOptionalFields = True
        End If
    Next

    'Display optional fields if any
    If displayOptionalFields Then
        Dim ifoptional As StringBuilder = New StringBuilder()
        ifoptional.Append("<div class="" well""> ")

        For Each item In optionalFields
            If data(item.Key) <> "" Then
                ifoptional.Append("<div class=""form-group""><label for=""" & item.Key & """ class=""cols-sm-3 control-label"">" & item.Key)
                ifoptional.Append(" "" class=""col-sm-3 control-label"">")
                ifoptional.Append(item.Key)
                ifoptional.Append(" </label><p id=""")
                ifoptional.Append(item.Key)
                ifoptional.Append(""" class=""form-control-static"">")
                ifoptional.Append(data(item.Key))
                ifoptional.Append("</p></div>")
            End If
        Next

        ifoptional.Append("</div>")
        optionalHtml = Html.Raw(ifoptional.ToString)
    End If

    Dim encryptionKey = Request("encryption_key")

    Dim pw3 = New PaygatePayweb3()
    pw3.setEncryptionKey(encryptionKey)
    pw3.setInitiateRequest(data)

    Dim checksum = pw3.generateChecksum(data)

    'Initiate request And check for valid response
    Dim returnData = pw3.doInitiate()

    Dim results As StringBuilder = New StringBuilder()
    Dim resultsHtml As String = ""
    Dim resultsForm As StringBuilder = New StringBuilder()
    Dim resultsFormHtml = Html.Raw("")

    If returnData Then  'Have a valid response
        If (pw3.lastError Is Nothing) Then
            'There is no error - continue
            For Each item In pw3.processRequest
                results.Append((item.Key + (" = " _
                                + (item.Value + "" & vbLf))))
            Next
            resultsHtml = results.ToString
        End If

        If (pw3.lastError Is Nothing) Then
            'There is no error - continue
            'Check checksums match
            Dim isValid = pw3.validateChecksum(pw3.initiateResponse)
            If isValid Then
                For Each item In pw3.processRequest
                    resultsForm.Append("<input type=""hidden"" name=""")
                    resultsForm.Append(item.Key)
                    resultsForm.Append(""" value=""")
                    resultsForm.Append(item.Value)
                    resultsForm.Append("""/>")
                Next
                resultsFormHtml = Html.Raw(resultsForm.ToString)
            Else
                resultsForm.Append("The checksums do not match")
                resultsFormHtml = Html.Raw(resultsForm.ToString)
            End If

        End If

    Else
        resultsHtml = "There was an error in the request. Please check your data."
    End If
End Code

<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayWeb 3 - Request</title>
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
                            <a href="index.cshtml">Initiate</a>
                        </li>
                        <li>
                            <a href="query.cshtml">Query</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
        <div style="background-color:#80b946; text-align: center; margin-top: 51px; margin-bottom: 15px; padding: 4px;"><strong>Step: Request / Redirect</strong></div>
        <div class="container">
            <form role="form" class="form-horizontal text-left" action="@PaygatePayweb3.process_url" method="post" name="paygate_process_form">
                <div class="form-group">
                    <label for="PAYGATE_ID" class="col-sm-3 control-label">PayGate ID</label>
                    <p id="PAYGATE_ID" class="form-control-static">@data("PAYGATE_ID")</p>
                </div>
                <div class="form-group">
                    <label for="REFERENCE" class="col-sm-3 control-label">Reference</label>
                    <p id="REFERENCE" class="form-control-static">@data("REFERENCE")</p>
                </div>
                <div class="form-group">
                    <label for="AMOUNT" class="col-sm-3 control-label">Amount</label>
                    <p id="AMOUNT" class="form-control-static">@data("AMOUNT")</p>
                </div>
                <div class="form-group">
                    <label for="CURRENCY" class="col-sm-3 control-label">Currency</label>
                    <p id="CURRENCY" class="form-control-static">@data("CURRENCY")</p>
                </div>
                <div class="form-group">
                    <label for="RETURN_URL" class="col-sm-3 control-label">Return URL</label>
                    <p id="RETURN_URL" class="form-control-static">@data("RETURN_URL")</p>
                </div>
                <div class="form-group">
                    <label for="LOCALE" class="col-sm-3 control-label">Locale</label>
                    <p id="LOCALE" class="form-control-static">@data("LOCALE")</p>
                </div>
                <div class="form-group">
                    <label for="COUNTRY" class="col-sm-3 control-label">Country</label>
                    <p id="COUNTRY" class="form-control-static">@data("COUNTRY")</p>
                </div>
                <div class="form-group">
                    <label for="TRANSACTION_DATE" class="col-sm-3 control-label">Transaction Date</label>
                    <p id="TRANSACTION_DATE" class="form-control-static">@data("TRANSACTION_DATE")</p>
                </div>
                <div class="form-group">
                    <label for="EMAIL" class="col-sm-3 control-label">Customer Email</label>
                    <p id="EMAIL" class="form-control-static">@data("EMAIL")</p>
                </div>
                @optionalHtml
                <div class="form-group">
                    <label for="encryption_key" class="col-sm-3 control-label">Encryption Key</label>
                    <p id="encryption_key" class="form-control-static">@encryptionKey</p>
                </div>
                <div class="form-group">
                    <label for="request">Request Result</label><br>
                    <textarea class="form-control" rows="3" cols="50" id="request">@resultsHtml</textarea>
                    <p>@resultsFormHtml</p>
                </div>

                <br>
                <div class="form-group">
                    <div class=" col-sm-offset-4 col-sm-4">
                        <input class="btn btn-success btn-block" type="submit" name="btnSubmit" value="Redirect" />
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
