@*
    Does the same as request.vbhtml
    Only display format is changed
*@

@Imports System.Text.RegularExpressions
@Imports System.Text
@Code
    Dim processUrl = PaygatePayweb3.process_url
    Dim optionalHtml = Html.Raw("")

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

    SessionModel.pgid = mandatoryFields("PAYGATE_ID")
    SessionModel.reference = mandatoryFields("REFERENCE")
    SessionModel.key = Request("encryption_key")

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

    If displayOptionalFields Then
        Dim ifoptional As StringBuilder = New StringBuilder()

        For Each item In optionalFields
            If data(item.Key) <> "" Then
                ifoptional.Append("<label for=""" & item.Key & """>" & item.Key)
                ifoptional.Append(" </label><span id=""")
                ifoptional.Append(item.Key)
                ifoptional.Append(""">")
                ifoptional.Append(data(item.Key))
                ifoptional.Append("</span><br>")
            End If
        Next
        optionalHtml = Html.Raw(ifoptional.ToString)
    End If

    Dim encryption_Key = Request("encryption_key")

    Dim pw3 = New PaygatePayweb3()
    pw3.setEncryptionKey(encryption_Key)
    pw3.setInitiateRequest(data)

    Dim checksum = pw3.generateChecksum(data)

    Dim returnData = pw3.doInitiate()

    Dim results As StringBuilder = New StringBuilder()
    Dim resultsHtml As String = ""
    Dim resultsForm As StringBuilder = New StringBuilder()
    Dim resultsFormHtml = Html.Raw("")

    If returnData Then
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
    <title>PayWeb 3 - Request</title>
    <style type="text/css">
        label {
            margin-top: 5px;
            display: inline-block;
            width: 150px;
        }
    </style>
</head>
<body>
    <form action="@PaygatePayweb3.process_url" method="post" name="paygate_process_form">
        <label for="PAYGATE_ID">PayGate ID</label>
        <span id="PAYGATE_ID">@data("PAYGATE_ID")</span>
        <br>
        <label for="REFERENCE"> Reference</label>
        <span id="REFERENCE">@data("REFERENCE")</span>
        <br>
        <label for="AMOUNT"> Amount</label>
        <span id="AMOUNT">@data("AMOUNT")</span>
        <br>
        <label for="CURRENCY"> Currency</label>
        <span id="CURRENCY">@data("CURRENCY")</span>
        <br>
        <label for="RETURN_URL">Return URL</label>
        <span id="RETURN_URL">@data("RETURN_URL")</span>
        <br>
        <label for="LOCALE"> Locale</label>
        <span id="LOCALE">@data("LOCALE")</span>
        <br>
        <label for="COUNTRY"> Country</label>
        <span id="COUNTRY">@data("COUNTRY")</span>
        <br>
        <label for="TRANSACTION_DATE"> Transaction Date</label>
        <span id="TRANSACTION_DATE">@data("TRANSACTION_DATE")</span>
        <br>
        <label for="EMAIL"> Customer Email</label>
        <span id="EMAIL">@data("EMAIL")</span>
        <br> @optionalHtml
        <label for="encryption_key">Encryption Key</label>
        <span id="encryption_key">@encryption_key</span>
        <br>
        <label style="vertical-align: top;" for="response">RESPONSE</label>
        <textarea class="form-control" rows="5" cols="100" id="response">@resultsHtml</textarea>
        <p>@resultsFormHtml</p>
        <br>
        <input class="btn btn-success btn-block" type="submit" name="btnSubmit" value="Redirect" />
        <br>
    </form>
</body>
</html>