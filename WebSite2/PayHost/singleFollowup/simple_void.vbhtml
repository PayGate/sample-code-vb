@*
    Void transaction example - simple format
*@

@Code
    'Key variables that are mandatory
    Dim pgid = PayhostSOAP.DEFAULT_PGID
    Dim reference = GlobalUtility.generateReference()
    Dim encryptionKey = PayhostSOAP.DEFAULT_ENCRYPTION_KEY
    Dim transId = ""
    Dim merchantOrderId = ""
    Dim voidResponse = ""
    Dim voidRequestText = ""

    'PayHost Web Service
    Dim payHOSTT As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

    'Make a SingleFollowUp call
    Dim voidRequest As PayHost.VoidRequestType = New PayHost.VoidRequestType()
    voidRequest.Account = New PayHost.PayGateAccountType()
    voidRequest.Account.PayGateId = pgid
    voidRequest.Account.Password = encryptionKey

    If (Not (Request("btnSubmit")) Is Nothing) Then
        Dim identifier = Request("identifier")
        Select Case (identifier)
            Case "merchantorderid"
                voidRequest.ItemElementName = New PayHost.ItemChoiceType2
                voidRequest.ItemElementName = PayHost.ItemChoiceType2.MerchantOrderId
                voidRequest.Item = Request("merchantOrderId")
            Case "transid"
                voidRequest.ItemElementName = New PayHost.ItemChoiceType2
                voidRequest.ItemElementName = PayHost.ItemChoiceType2.TransactionId
                voidRequest.Item = Request("transid")
        End Select

        voidRequest.TransactionType = New PayHost.TransactionType
        If ((Not (Request("transactionType")) Is Nothing) _
                    AndAlso (Request("transactionType") <> "")) Then
            voidRequest.TransactionType = CType([Enum].Parse(GetType(PayHost.TransactionType), Request("transactionType")), PayHost.TransactionType)
        End If

        voidRequestText = PayhostSOAP.getXMLText(voidRequest)

        Try
            Dim fupRequest = New PayHost.SingleFollowUpRequest With {
                .Item = voidRequest
            }
            Dim response = payHOSTT.SingleFollowUp(New PayHost.SingleFollowUpRequest1(fupRequest))
            Dim t = TryCast(response.SingleFollowUpResponse, PayHost.SingleFollowUpResponse)
            Dim status = TryCast(t.Items(0), PayHost.VoidResponseType)
            voidResponse = PayhostSOAP.getXMLText(response)
        Catch e As Exception
            Dim err = e.Message
        End Try
    End If
End Code

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayHost - Void</title>
    <style type="text/css">
        label {
            margin-top: 5px;
            display: inline-block;
            width: 150px;
        }
    </style>
</head>
<body>
    <a href="../singleFollowUp/void.vbhtml">Back to Void</a>
    <br />
    <form role="form" class="form-horizontal text-left" action="simple_void.vbhtml" method="post">
        <label for="payGateId" class="col-sm-3 control-label">PayGate ID</label>
        <input class="form-control" type="text" name="payGateId" id="payGateId" value="@pgid" /><br />
        <label for="encryptionKey" class="col-sm-3 control-label">Encryption Key</label>
        <input class="form-control" type="text" name="encryptionKey" id="encryptionKey" value="@encryptionKey" /><br />
        <label for="transactionType" class="col-sm-3 control-label">Transaction Type</label>
        <select name="transactionType" id="transactionType" class="form-control">
            <option value="Authorisation">Authorisation</option>
            <option value="Settlement">Settlement</option>
            <option value="Refund">Refund</option>
            <option value="Payout">Payout</option>
            <option value="Purchase">Purchase</option>
        </select><br />
        <span class="input-group-addon">
            <label for="merchantOrderIdChk" class="sr-only">Merchant Order ID</label>
            <input name="identifier" id="merchantOrderIdChk" value="merchantorderid" type="radio" aria-label="Merchant Order ID Checkbox">
        </span>
        <input type="text" name="merchantOrderId" id="merchantOrderId" class="form-control" aria-label="Merchant Order ID Input" value="@merchantOrderId" /><br />
        <span class="input-group-addon">
            <label for="transidChk" class="sr-only">Transaction ID Checkbox</label>
            <input name="identifier" id="transidChk" value="transid" type="radio" aria-label="Transaction ID Checkbox">
        </span>
        <input type="text" name="transId" id="transId" class="form-control" aria-label="Transaction ID Input" value="@transId" />
        <br>
        <div class="form-group">
            <div class=" col-sm-offset-4 col-sm-4">
                <img src="/../../lib/images/loader.gif" alt="Processing" class="initialHide" id="queryLoader">
                <input class="btn btn-success btn-block" id="doVoidBtn" type="submit" name="btnSubmit" value="Do Void" />
            </div>
        </div>
        <br>
    </form>
    <label style="vertical-align: top;" for="request">REQUEST: </label>
    <textarea rows="20" cols="100" id="request" class="form-control">@voidRequestText</textarea><br />
    <label style="vertical-align: top;" for="response">RESPONSE: </label>
    <textarea rows="20" cols="100" id="response" class="form-control">@voidResponse</textarea>
</body>
</html>
