@*
    Does the same as query.vbhtml
    Only display format is changed
*@

@Imports System.Text
@Code
    Dim data = New Dictionary(Of String, String)() From {
        {"PAYGATE_ID", Request("PAYGATE_ID")},
        {"PAY_REQUEST_ID", Request("PAY_REQUEST_ID")},
        {"REFERENCE", Request("REFERENCE")}
    }

    Dim encryption_key = If(Request("encryption_key") <> "", Request("encryption_key"), "secret")
    Dim pgid = If(data("PAYGATE_ID") <> "", data("PAYGATE_ID"), "10011072130")
    Dim payrequestid = If(data("PAY_REQUEST_ID") <> "", data("PAY_REQUEST_ID"), "")
    Dim reference = If(data("REFERENCE") <> "", data("REFERENCE"), "")

    Dim pg3 = New PaygatePayweb3()
    pg3.setEncryptionKey(encryption_key)
    pg3.setQueryRequest(data)
    Dim returnData = pg3.doQuery()

    Dim htmlError = New StringBuilder("<label for=""response"">RESPONSE: </label><br>")

    If pg3.queryResponse.Count > 0 Then

        If pg3.lastError Is Nothing Then
            htmlError.Append("<textarea name=""response"" id=""response"" rows=""20"" cols=""100"">")
            For Each item In pg3.queryResponse
                htmlError.Append(item.Key & " = " & item.Value & vbNewLine)
            Next
            htmlError.Append("</textarea>")
        Else
            htmlError.Append("ERROR: " & pg3.lastError)
        End If
    End If

    htmlError.Append("</div>")
    Dim htmlErrorRaw = Html.Raw(htmlError)
End Code


<html>
<head>
    <title>PayWeb 3 - Query</title>
    <style type="text/css">

        label {
            margin-top: 5px;
            display: inline-block;
            width: 200px;
        }
    </style>
</head>
<body>
    <a href="query.vbhtml">Back to Query</a>
    <form action="simple_query.vbhtml" method="post">
        <label for="PAYGATE_ID" class="col-sm-3 control-label">PayGate ID</label>
        <input type="text" name="PAYGATE_ID" id="PAYGATE_ID" class="form-control" value="@pgid" />
        <br>
        <label for="PAY_REQUEST_ID" class="col-sm-3 control-label">Pay Request ID</label>
        <input type="text" name="PAY_REQUEST_ID" id="PAY_REQUEST_ID" class="form-control" value="@payrequestid" />
        <br>
        <label for="REFERENCE" class="col-sm-3 control-label">Reference</label>
        <input type="text" name="REFERENCE" id="REFERENCE" class="form-control" value="@reference" />
        <br>
        <label for="encryption_key" class="col-sm-3 control-label">Encryption Key</label>
        <input type="text" name="encryption_key" id="encryption_key" class="form-control" value="@encryption_key" />
        <br>
        <br>
        <input type="submit" id="doQueryBtn" class="btn btn-success btn-block" value="Do Query" name="btnSubmit">
        <br>
    </form>
    @htmlErrorRaw
</body>
</html>