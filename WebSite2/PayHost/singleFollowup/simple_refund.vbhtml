@*
    Refund transaction example - simple format
*@

@Imports System.Globalization
@Code
    'Mandatory key variables
    Dim pgid = PayhostSOAP.DEFAULT_PGID
    Dim reference = GlobalUtility.generateReference()
    Dim encryptionKey = PayhostSOAP.DEFAULT_ENCRYPTION_KEY
    Dim transId = ""
    Dim merchantOrderId = ""
    Dim refundResponse = ""
    Dim refundRequestText = ""
    Dim amount As String = ""

    'PayHost Web Service
    Dim payHOSTT As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

    'Will make a SingleFollowUp call
    Dim refundRequest As PayHost.RefundRequestType = New PayHost.RefundRequestType()
    refundRequest.Account = New PayHost.PayGateAccountType()
    refundRequest.Account.PayGateId = pgid
    refundRequest.Account.Password = encryptionKey

    If ((Not (Request("amount")) Is Nothing) _
            AndAlso (Request("amount") <> "")) Then
        'Replace decimal comma with decimal point
        amount = Request("amount").Replace(",", ".")
        'And convert to cents
        refundRequest.Amount = CType((100 * Decimal.Parse(Request("amount"), CultureInfo.InvariantCulture)), Integer)
    End If

    If Request("btnSubmit") IsNot Nothing Then
        Dim identifier = Request("identifier")

        Select Case identifier
            Case "merchantorderid"
                refundRequest.ItemElementName = New PayHost.ItemChoiceType1()
                refundRequest.ItemElementName = PayHost.ItemChoiceType1.MerchantOrderId
                refundRequest.Item = Request("merchantOrderId")
            Case "transid"
                refundRequest.ItemElementName = New PayHost.ItemChoiceType1()
                refundRequest.ItemElementName = PayHost.ItemChoiceType1.TransactionId
                refundRequest.Item = Request("transid")
        End Select

        refundRequestText = PayhostSOAP.getXMLText(refundRequest)

        Try
            Dim fupRequest = New PayHost.SingleFollowUpRequest With {
                .Item = refundRequest
            }
            Dim response = payHOSTT.SingleFollowUp(New PayHost.SingleFollowUpRequest1(fupRequest))
            Dim t = TryCast(response.SingleFollowUpResponse, PayHost.SingleFollowUpResponse)
            Dim status = TryCast(t.Items(0), PayHost.RefundResponseType)
            refundResponse = PayhostSOAP.getXMLText(response)
        Catch e As Exception
            Dim err = e.Message
        End Try
    End If
End Code

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayHost - Refund</title>
    <style type="text/css">
        label {
            margin-top: 5px;
            display: inline-block;
            width: 150px;
        }
    </style>
</head>
<body>
    <a href="../singleFollowUp/refund.vbhtml">Back to Refund</a>
    <br />
    <form role="form" class="form-horizontal text-left" action="simple_refund.vbhtml" method="post">
        <label for="payGateId" class="col-sm-3 control-label">PayGate ID</label>
        <input class="form-control" type="text" name="payGateId" id="payGateId" value="@pgid" /><br />
        <label for="encryptionKey" class="col-sm-3 control-label">Encryption Key</label>
        <input class="form-control" type="text" name="encryptionKey" id="encryptionKey" value="@encryptionKey" /><br />
        <label for="amount" class="col-sm-3 control-label">Amount</label>
        <input type="text" name="amount" id="amount" class="form-control" aria-label="amount input" value="@amount" /><br />
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
                <input class="btn btn-success btn-block" id="doVoidBtn" type="submit" name="btnSubmit" value="Do Refund" />
            </div>
        </div>
        <br>
    </form>
    <label style="vertical-align: top;" for="request">REQUEST: </label>
    <textarea rows="20" cols="100" id="request" class="form-control">@refundRequestText</textarea><br />
    <label style="vertical-align: top;" for="response">RESPONSE: </label>
    <textarea rows="20" cols="100" id="response" class="form-control">@refundResponse</textarea>
</body>
</html>
