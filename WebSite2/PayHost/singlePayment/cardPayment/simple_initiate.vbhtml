@*
    This is an example page of the process required for a PayGate PayHost Card Payment transaction.
    Same functionality as index.vbhtml, simpler display
*@

@Imports System.Text
@Code
    Dim initRequest = New StringBuilder()
    Dim responseTxt As String = ""
    Dim fullPath = GlobalUtility.getUriParts()
    Dim queryResponse = New PayHost.SinglePaymentResponse1()
    Dim r1 = New PayHost.CardPaymentResponseType()
    Dim htmlRedirectRaw = Html.Raw("")
    Dim htmlRedirectRaw1 = Html.Raw("")
    Dim htmlRedirectRaw2 = Html.Raw("")
    Dim htmlRedirect = ""
    Dim redirectUrl As String = ""

    'SSL/TLS connection
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

    'Set up Payhost Web Service
    Dim payHOSTT As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")

    'Set initial form parameters
    Dim payGateId = "10011072130"
    Dim encryptionKey = "test"
    Dim reference = GlobalUtility.generateReference()

    'Set session variables
    SessionModel.pgid = payGateId
    SessionModel.reference = reference
    SessionModel.key = encryptionKey

    'Set variables for request
    Dim cardPaymentRequestType As PayHost.CardPaymentRequestType = New PayHost.CardPaymentRequestType()
    cardPaymentRequestType.Account = New PayHost.PayGateAccountType()
    cardPaymentRequestType.Account.Password = encryptionKey
    cardPaymentRequestType.Account.PayGateId = payGateId
    cardPaymentRequestType.Items = New String(1) {}

    'Customer fields are optional
    cardPaymentRequestType.Customer = New PayHost.PersonType()
    cardPaymentRequestType.Customer.Title = "Mr"
    cardPaymentRequestType.Customer.FirstName = "PayGate"
    cardPaymentRequestType.Customer.LastName = "Test"
    cardPaymentRequestType.Customer.Telephone = GlobalUtility.getStringArray("0861234567")
    cardPaymentRequestType.Customer.Mobile = GlobalUtility.getStringArray("0842573344")
    cardPaymentRequestType.Customer.Fax = GlobalUtility.getStringArray("08600999111")
    cardPaymentRequestType.Customer.Email = GlobalUtility.getStringArray("itsupport@paygate.co.za")

    'Card details - follow the given order to avoid validation error exceptions
    cardPaymentRequestType.ItemsElementName = New PayHost.ItemsChoiceType() {PayHost.ItemsChoiceType.CardNumber, PayHost.ItemsChoiceType.CardExpiryDate}
    cardPaymentRequestType.Items = New String() {"4000000000000002", "122020"}
    cardPaymentRequestType.CardIssueDate = "122016"
    cardPaymentRequestType.CardIssueNumber = "345"
    cardPaymentRequestType.CVV = "987"
    cardPaymentRequestType.BudgetPeriod = "0"

    'Redirects
    cardPaymentRequestType.Redirect = New PayHost.RedirectRequestType()
    cardPaymentRequestType.Redirect.NotifyUrl = fullPath(0) & "//" + fullPath(2) & "/PayHost/notify.vbhtml"
    cardPaymentRequestType.Redirect.ReturnUrl = fullPath(0) & "//" + fullPath(2) & "/PayHost/result.vbhtml"

    'Order details
    cardPaymentRequestType.Order = New PayHost.OrderType()
    cardPaymentRequestType.Order.MerchantOrderId = GlobalUtility.generateReference()
    cardPaymentRequestType.Order.Currency = PayHost.CurrencyType.ZAR
    cardPaymentRequestType.Order.Amount = 12311
    cardPaymentRequestType.Order.OrderItems = New PayHost.OrderItemType(0) {}
    cardPaymentRequestType.Order.OrderItems(0) = New PayHost.OrderItemType()
    cardPaymentRequestType.Order.OrderItems(0).ProductCode = "ABC123"
    cardPaymentRequestType.Order.OrderItems(0).ProductDescription = "Misc"
    cardPaymentRequestType.Order.OrderItems(0).ProductCategory = "misc"
    cardPaymentRequestType.Order.OrderItems(0).ProductRisk = "XX"
    cardPaymentRequestType.Order.OrderItems(0).OrderQuantity = UInt32.Parse("1")
    cardPaymentRequestType.Order.OrderItems(0).OrderQuantitySpecified = True
    cardPaymentRequestType.Order.OrderItems(0).UnitPrice = Decimal.Parse("123.11")
    cardPaymentRequestType.Order.OrderItems(0).UnitPriceSpecified = True
    cardPaymentRequestType.Order.OrderItems(0).Currency = "ZAR"

    'Representation of the above request - for info only, not used in request
    Dim initRequestTxt = PayhostSOAP.getXMLText(cardPaymentRequestType)

    If Request("btnSubmit") IsNot Nothing Then

        Try
            Dim sprequest = New PayHost.SinglePaymentRequest With {
                .Item = cardPaymentRequestType
            }
            Dim sprequest1 = New PayHost.SinglePaymentRequest1(sprequest)
            queryResponse = payHOSTT.SinglePayment(sprequest1)
            r1 = TryCast(queryResponse.SinglePaymentResponse.Item, PayHost.CardPaymentResponseType)
            responseTxt = PayhostSOAP.getXMLText(r1)
            redirectUrl = r1.Redirect.RedirectUrl
            Dim htmlRedirect1 = "<form role=""form"" class=""form-horizontal text-left"" action="""
            htmlRedirect1 += r1.Redirect.RedirectUrl & """ method=""post"" target=""iframetest"">"
            htmlRedirect1 = ""

            For Each item In r1.Redirect.UrlParams
                htmlRedirect1 += "<input type=""hidden"" name=""" & item.key & """ value=""" + item.value & """/>" & Environment.NewLine
            Next

            htmlRedirectRaw1 = Html.Raw(htmlRedirect1)

            If r1.Redirect IsNot Nothing Then
                htmlRedirect = "<div class=""row""><div class=""col-sm-offset-4 col-sm-4"">"
                htmlRedirect += "<button id=""showRedirectBtn"" class=""btn btn-warning btn-block"" type=""button"" data-toggle=""collapse"" data-target=""#redirectDiv"" aria-expanded=""false"" aria-controls=""redirectDiv"">Redirect Required</button></div></div><br>"
                htmlRedirect += "<div id=""redirectDiv"" class=""collapse well well-sm""><div class=""row""><div class=""col-sm-12"">"
                htmlRedirect += "<label for=""response"" class=""col-sm-3 sr-only"">Redirect</label>"
                htmlRedirect += "<textarea class=""form-control"" rows=""8"" cols=""100"" id=""redirect"">"
                'htmlRedirect += Html.Raw(htmlRedirect1)
                htmlRedirect += (htmlRedirect1)
                htmlRedirect += "</textarea></div></div>"
            End If

            htmlRedirectRaw = Html.Raw(htmlRedirect)
            Dim htmlRedirect2 = "<form role=""form"" class=""form-horizontal text-left"" action=""" & redirectUrl & """ method=""post"" target=""iframetest"">" & htmlRedirect1
            htmlRedirect2 += "<br><div class=""form-group""><div class=""col-sm-offset-4 col-sm-4""><img src=""../../../lib/images/loader.gif"" alt=""Processing"" class=""initialHide"" id=""submitLoader"">"
            htmlRedirect2 += "<input class=""btn btn-success btn-block"" type=""submit"" name=""submitBtn"" id=""doSubmitBtn"" value=""submit"" data-toggle=""modal"" data-target=""#authModal"" />"
            htmlRedirect2 += "</div></div><br></form>"
            htmlRedirectRaw2 = Html.Raw(htmlRedirect2)
        Catch sf As Exception
            Dim err = sf.Message
        End Try
    Else
    End If

End Code

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayHost - CardPayment</title>
    <style type="text/css">
        label {
            margin-top: 5px;
            display: inline-block;
            width: 150px;
        }
    </style>
</head>
<body>
    <a href="index.vbhtml">back to Initiate</a>
    <form action="simple_initiate.vbhtml" method="post">
        <label style="vertical-align: top;" for="request">Request</label>
        <textarea name="request" cols="130" rows="45" id="request">
            @initRequestTxt
		</textarea>
        <br>
        <input id="doAuthBtn" type="submit" name="btnSubmit" value="Do Auth" />
        <br>
        <label style="vertical-align: top;" for="response">Response</label>
        <textarea name="response" cols="100" rows="10">
            @responseTxt
        </textarea>
    </form>
    @htmlRedirectRaw
    @htmlRedirectRaw2
</body>
</html>
