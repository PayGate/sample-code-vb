@Code
    'Key variables that are mandatory
    Dim pgid = PayhostSOAP.DEFAULT_PGID
    Dim reference = GlobalUtility.generateReference()
    Dim encryptionKey = PayhostSOAP.DEFAULT_ENCRYPTION_KEY
    Dim payrequestId = ""
    Dim transId = ""
    Dim queryResponse = ""
    Dim queryRequestText = ""

    'Payhost Web Service
    Dim payHOSTT As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

    'Make a SingleFollowup call
    Dim queryRequest As PayHost.QueryRequestType = New PayHost.QueryRequestType()
    queryRequest.Account = New PayHost.PayGateAccountType()
    queryRequest.Account.PayGateId = pgid
    queryRequest.Account.Password = encryptionKey

    If Request("btnSubmit") IsNot Nothing Then
        Dim identifier = Request("identifier")

        Select Case identifier
            Case "payrequestid"
                queryRequest.ItemsElementName = New PayHost.ItemsChoiceType2() {PayHost.ItemsChoiceType2.PayRequestId}
                queryRequest.Items = New String() {Request("payrequestid")}
            Case "reference"
                queryRequest.ItemsElementName = New PayHost.ItemsChoiceType2() {PayHost.ItemsChoiceType2.MerchantOrderId}
                queryRequest.Items = New String() {Request("reference")}
            Case "transid"
                queryRequest.ItemsElementName = New PayHost.ItemsChoiceType2() {PayHost.ItemsChoiceType2.TransactionId}
                queryRequest.Items = New String() {Request("transid")}
        End Select

        queryRequestText = PayhostSOAP.getXMLText(queryRequest)

        Try
            Dim fupRequest = New PayHost.SingleFollowUpRequest With {
                .Item = queryRequest
            }
            Dim response = payHOSTT.SingleFollowUp(New PayHost.SingleFollowUpRequest1(fupRequest))
            Dim t = TryCast(response.SingleFollowUpResponse, PayHost.SingleFollowUpResponse)
            Dim status = TryCast(t.Items(0), PayHost.QueryResponseType)
            queryResponse = PayhostSOAP.getXMLText(status)
        Catch e As Exception
            Dim err = e.Message
        End Try
    End If

End Code

<html>
<head>
    <title>PayHost - Query</title>
    <style type="text/css">
        label {
            margin-top: 5px;
            display: inline-block;
            width: 150px;
        }
    </style>
</head>
<body>
    <a href="../singleFollowUp/query.vbhtml">Back to Query</a>
    <br>
    <form role="form" class="form-horizontal text-left" action="simple_query.vbhtml" method="post">
        <label for="payGateId">PayGate ID</label>
        <input type="text" name="payGateId" id="payGateId" value="@pgid" />
        <br>
        <label for="encryptionKey" class="col-sm-3 control-label">Encryption Key</label>
        <input class="form-control" type="text" name="encryptionKey" id="encryptionKey" value="@encryptionKey" />
        <br>
        <label for="payrequestidChk" class="sr-only">Use Pay Request ID</label>
        <input name="identifier" id="payrequestidChk" value="payrequestid" type="radio" aria-label="Pay Request ID Checkbox">
        <br>
        <label for="transidChk" class="sr-only">Use Transaction ID</label>
        <input name="identifier" id="transidChk" value="transid" type="radio" aria-label="Transaction ID Checkbox">
        <br>
        <label for="referenceChk" class="sr-only">Use Reference</label>
        <input name="identifier" id="referenceChk" value="reference" type="radio" aria-label="Reference Checkbox" checked />
        <br>
        <label for="payRequestId" class="col-sm-3 control-label">Pay Request ID</label>
        <input type="text" name="payRequestId" id="payRequestId" class="form-control" aria-label="Pay Request ID Input" value="@payrequestId" />
        <br>
        <label for="transId" class="col-sm-3 control-label">Transaction ID</label>
        <input type="text" name="transId" id="transId" class="form-control" aria-label="Transaction ID Input" value="@transId" />
        <br>
        <label for="reference" class="col-sm-3 control-label">Reference</label>
        <input type="text" name="reference" id="reference" class="form-control" aria-label="Reference Input" value="@reference">
        <br>
        <input id="doQueryBtn" type="submit" name="btnSubmit" value="Do Query" />
    </form>
    <label style="vertical-align: top;" for="request">REQUEST:</label>
    <textarea rows="20" cols="100" id="request">@queryRequestText</textarea><br>
    <label style="vertical-align: top;" for="response">RESPONSE:</label>
    <textarea rows="20" cols="100" id="response">@queryResponse</textarea>
</body>
</html>