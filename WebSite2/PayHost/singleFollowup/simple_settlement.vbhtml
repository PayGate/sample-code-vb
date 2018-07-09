@*
    Settlement transaction example - simple format
*@

@Code
    'Key variables that are mandatory
    Dim pgid = PayhostSOAP.DEFAULT_PGID
    Dim reference = GlobalUtility.generateReference()
    Dim encryptionKey = PayhostSOAP.DEFAULT_ENCRYPTION_KEY
    Dim transId = ""
    Dim merchantOrderId = ""
    Dim settleResponse = ""
    Dim settleRequestText = ""

    'PayHost Web Service
    Dim payHOSTT As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

    'Will make a SingleFollowUp call
    Dim settleRequest As PayHost.SettleRequestType = New PayHost.SettleRequestType()
    settleRequest.Account = New PayHost.PayGateAccountType()
    settleRequest.Account.PayGateId = pgid
    settleRequest.Account.Password = encryptionKey

    If Request("btnSubmit") IsNot Nothing Then
        Dim identifier = Request("identifier")

        Select Case identifier
            Case "merchantorderid"
                settleRequest.ItemElementName = New PayHost.ItemChoiceType()
                settleRequest.ItemElementName = PayHost.ItemChoiceType.MerchantOrderId
                settleRequest.Item = Request("merchantOrderId")
            Case "transid"
                settleRequest.ItemElementName = New PayHost.ItemChoiceType()
                settleRequest.ItemElementName = PayHost.ItemChoiceType.TransactionId
                settleRequest.Item = Request("transid")
        End Select

        settleRequestText = PayhostSOAP.getXMLText(settleRequest)

        Try
            Dim fupRequest = New PayHost.SingleFollowUpRequest With {
                .Item = settleRequest
            }
            Dim response = payHOSTT.SingleFollowUp(New PayHost.SingleFollowUpRequest1(fupRequest))
            Dim t = TryCast(response.SingleFollowUpResponse, PayHost.SingleFollowUpResponse)
            Dim status = TryCast(t.Items(0), PayHost.SettleResponseType)
            settleResponse = PayhostSOAP.getXMLText(response)
        Catch e As Exception
            Dim err = e.Message
        End Try
    End If
End Code

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayHost - Settlement</title>
    <style type="text/css">
        label {
            margin-top: 5px;
            display: inline-block;
            width: 150px;
        }
    </style>
</head>
<body>
    <a href="../singleFollowUp/settlement.vbhtml">Back to Settlement</a>
    <br />
    <form role="form" class="form-horizontal text-left" action="simple_settlement.vbhtml" method="post">
        <label for="payGateId" class="col-sm-3 control-label">PayGate ID</label>
        <input class="form-control" type="text" name="payGateId" id="payGateId" value="@pgid" /><br />
        <label for="encryptionKey" class="col-sm-3 control-label">Encryption Key</label>
        <input class="form-control" type="text" name="encryptionKey" id="encryptionKey" value="@encryptionKey" /><br />
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
                <input class="btn btn-success btn-block" id="doVoidBtn" type="submit" name="btnSubmit" value="Do Settlement" />
            </div>
        </div>
        <br>
    </form>
    <label style="vertical-align: top;" for="request">REQUEST: </label>
    <textarea rows="20" cols="100" id="request" class="form-control">@settleRequestText</textarea><br />
    <label style="vertical-align: top;" for="response">RESPONSE: </label>
    <textarea rows="20" cols="100" id="response" class="form-control">@settleResponse</textarea>
</body>
</html>
