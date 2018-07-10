@*
    This is an example page of the process required for a PayGate PayHost Card Token Payment transaction.
    Simple format
*@

@Imports System.Text
@Imports System.Globalization
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
    Dim amount = "123,45"

    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

    'PayHost Web Service
    Dim payHOSTT As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")
    Dim payGateId = "10011064270"
    Dim encryptionKey = "test"
    Dim reference = GlobalUtility.generateReference()

    'Set session variables
    SessionModel.pgid = payGateId
    SessionModel.reference = reference
    SessionModel.key = encryptionKey

    'Set request type
    Dim CardPaymentRequestType As PayHost.CardPaymentRequestType = New PayHost.CardPaymentRequestType()

    CardPaymentRequestType.Account = New PayHost.PayGateAccountType()
    CardPaymentRequestType.Account.Password = encryptionKey
    CardPaymentRequestType.Account.PayGateId = payGateId

    'Customer fields are optional
    CardPaymentRequestType.Customer = New PayHost.PersonType()
    CardPaymentRequestType.Customer.Title = "Mr"
    CardPaymentRequestType.Customer.FirstName = "PayGate"
    CardPaymentRequestType.Customer.LastName = "Test"
    CardPaymentRequestType.Customer.Telephone = GlobalUtility.getStringArray("0861234567")
    CardPaymentRequestType.Customer.Mobile = GlobalUtility.getStringArray("0842573344")
    CardPaymentRequestType.Customer.Fax = GlobalUtility.getStringArray("08600999111")
    CardPaymentRequestType.Customer.Email = GlobalUtility.getStringArray("itsupport@paygate.co.za")

    'Set vault id
    CardPaymentRequestType.ItemsElementName = New PayHost.ItemsChoiceType() {PayHost.ItemsChoiceType.VaultId}
    CardPaymentRequestType.Items = New String() {"33a58dd1-687d-4678-a3ab-8c8001be72cf"}

    'Set CVV
    CardPaymentRequestType.CVV = "123"

    'Redirects
    CardPaymentRequestType.Redirect = New PayHost.RedirectRequestType()
    CardPaymentRequestType.Redirect.NotifyUrl = fullPath(0) & "//" + fullPath(2) & "/PayHost/notify.vbhtml"
    CardPaymentRequestType.Redirect.ReturnUrl = fullPath(0) & "//" + fullPath(2) & "/PayHost/result.vbhtml"

    'Order details
    CardPaymentRequestType.Order = New PayHost.OrderType()
    CardPaymentRequestType.Order.MerchantOrderId = GlobalUtility.generateReference()
    CardPaymentRequestType.Order.Currency = PayHost.CurrencyType.ZAR

    'Convert decimal comma to decimap point
    amount = amount.Replace(",", ".")
    'Change to cents
    CardPaymentRequestType.Order.Amount = CType(100 * Decimal.Parse(amount, CultureInfo.InvariantCulture), Integer)

    CardPaymentRequestType.Order.OrderItems = New PayHost.OrderItemType(0) {}
    CardPaymentRequestType.Order.OrderItems(0) = New PayHost.OrderItemType()
    CardPaymentRequestType.Order.OrderItems(0).ProductCode = "ABC123"
    CardPaymentRequestType.Order.OrderItems(0).ProductDescription = "Misc"
    CardPaymentRequestType.Order.OrderItems(0).ProductCategory = "misc"
    CardPaymentRequestType.Order.OrderItems(0).ProductRisk = "XX"
    CardPaymentRequestType.Order.OrderItems(0).OrderQuantity = UInt32.Parse("1")
    CardPaymentRequestType.Order.OrderItems(0).OrderQuantitySpecified = True

    'Change to cents
    CardPaymentRequestType.Order.OrderItems(0).UnitPrice = CType(100 * Decimal.Parse(amount, CultureInfo.InvariantCulture), Integer)

    CardPaymentRequestType.Order.OrderItems(0).UnitPriceSpecified = True
    CardPaymentRequestType.Order.OrderItems(0).Currency = "ZAR"
    Dim xmltxt = PayhostSOAP.getXMLText(CardPaymentRequestType)
    Dim initRequestTxt = xmltxt

    If Request("btnSubmit") IsNot Nothing Then

        Try
            Dim sprequest = New PayHost.SinglePaymentRequest With {
                .Item = CardPaymentRequestType
            }
            Dim sprequest1 = New PayHost.SinglePaymentRequest1(sprequest)
            queryResponse = payHOSTT.SinglePayment(sprequest1)
            r1 = TryCast(queryResponse.SinglePaymentResponse.Item, PayHost.CardPaymentResponseType)
            responseTxt = PayhostSOAP.getXMLText(r1)
            Dim htmlRedirect1 = ""

            If r1.Redirect IsNot Nothing Then
                htmlRedirect1 = "<form role=""form"" class=""form-horizontal text-left"" action="""
                redirectUrl = r1.Redirect.RedirectUrl
                htmlRedirect1 += r1.Redirect.RedirectUrl & """ method=""post"" target=""iframetest"">"
                htmlRedirect1 = ""

                For Each item In r1.Redirect.UrlParams
                    htmlRedirect1 += "<input type=""hidden"" name=""" & item.key & """ value=""" + item.value & """/>" + Environment.NewLine
                Next
            End If

            htmlRedirectRaw1 = Html.Raw(htmlRedirect1)

            If r1.Redirect IsNot Nothing Then
                htmlRedirect = "<div class=""row""><div class=""col-sm-offset-4 col-sm-4"">"
                htmlRedirect += "<button id=""showRedirectBtn"" class=""btn btn-warning btn-block"" type=""button"" data-toggle=""collapse"" data-target=""#redirectDiv"" aria-expanded=""false"" aria-controls=""redirectDiv"">Redirect Required</button></div></div><br>"
                htmlRedirect += "<div id=""redirectDiv"" class=""collapse well well-sm""><div class=""row""><div class=""col-sm-12"">"
                htmlRedirect += "<label for=""response"" class=""col-sm-3 sr-only"">Redirect</label>"
                htmlRedirect += "<textarea class=""form-control"" rows=""8"" cols=""100"" id=""redirect"">"
                htmlRedirect += htmlRedirect1
                htmlRedirect += "</textarea></div></div>"
                htmlRedirectRaw = Html.Raw(htmlRedirect)
                Dim htmlRedirect2 = "<form role=""form"" class=""form-horizontal text-left"" action=""" & redirectUrl & """ method=""post"" target=""iframetest"">" & htmlRedirect1
                htmlRedirect2 += "<br><div class=""form-group""><div class=""col-sm-offset-4 col-sm-4""><img src=""../../../lib/images/loader.gif"" alt=""Processing"" class=""initialHide"" id=""submitLoader"">"
                htmlRedirect2 += "<input class=""btn btn-success btn-block"" type=""submit"" name=""submitBtn"" id=""doSubmitBtn"" value=""submit"" data-toggle=""modal"" data-target=""#authModal"" />"
                htmlRedirect2 += "</div></div><br></form>"
                htmlRedirectRaw2 = Html.Raw(htmlRedirect2)
            End If

        Catch sf As Exception
            Dim err = sf.Message
        End Try
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
