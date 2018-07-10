@*
    This is an example page of the process required for a PayGate PayHost Card Token Payment transaction.
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
    <title>PayHost - TokenCardPayment</title>
    <link rel="stylesheet" href="../../../lib/css/bootstrap.min.css">
    <link rel="stylesheet" href="../../../lib/css/core.css">
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
                        <img alt="PayGate" src="../../../lib/images/paygate_logo_mini.png" />
                    </a>
                    <span style="color: #f4f4f4; font-size: 18px; line-height: 45px; margin-right: 10px;"><strong>PayHost Token Card Payment</strong></span>
                </div>
                <div class="collapse navbar-collapse" id="navbar-collapse">
                    <ul class="nav navbar-nav">
                        <li class="active">
                            <a href="index.vbhtml">Initiate</a>
                        </li>
                        <li>
                            <a href="../../singleFollowUp/query.vbhtml">Query</a>
                        </li>
                        <li>
                            <a href="simple_initiate.vbhtml">Simple initiate</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
        <div style="background-color:#80b946; text-align: center; margin-top: 51px; margin-bottom: 15px; padding: 4px;"><strong>Initiate Token Card Payment</strong></div>
        <div class="container">
            <form role="form" class="form-horizontal text-left" action="index.vbhtml" method="post">
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <button class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#requestDiv" aria-expanded="false" aria-controls="requestDiv">
                            Request
                        </button>
                    </div>
                </div>
                <div id="requestDiv" class="collapse well well-sm">
                    <div class="form-group">
                        <div class="col-sm-12">
                            <label for="request" class="col-sm-2 sr-only">Request</label>
                            <textarea name="request" cols="130" rows="45" id="request" class="form-control">
                                @initRequestTxt
							</textarea>
                        </div>
                    </div>
                </div>
                <br>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <img src="../../../lib/images/loader.gif" alt="Processing" class="initialHide" id="authLoader">
                        <input class="btn btn-success btn-block" id="doAuthBtn" type="submit" name="btnSubmit" value="Do Auth" formaction="index.vbhtml" />
                        <input class="btn btn-success btn-block" id="showResultBtn" type="button" name="showResult" value="Show Results" data-toggle="collapse" data-target="#btnSubmitBlock" />
                    </div>
                </div>
                <br>
                <div id="btnSubmitBlock" class="collapse well well-sm">
                    <div class="form-group">
                        <div class="col-sm-offset-4 col-sm-4">
                            <button class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#responseDiv" aria-expanded="false" aria-controls="responseDiv">
                                Response
                            </button>
                        </div>
                    </div>
                    <div id="responseDiv" class="collapse well well-sm">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="response" class="col-sm-3 sr-only">Response</label>
                                <textarea name="response" cols="130" rows="8" id="response" class="form-control">
                                @responseTxt
							</textarea>
                            </div>
                        </div>
                    </div>
                    @htmlRedirectRaw
                </div>
            </form>
            @htmlRedirectRaw2
            <!-- Modal to house the iframe we use for 3D Secure authentication if returned from PayHost -->
            <div class="modal fade" id="authModal" tabindex="-1" role="dialog" aria-labelledby="3DAuthModal" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title" id="myModalLabel">3D Authentication</h4>
                        </div>
                        <div class="modal-body">
                            <iframe name="iframetest" id="iframetest" src="@redirectUrl" height="550" width="100%" frameborder="0" scrolling="auto"></iframe>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript" src="../../../lib/js/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="../../../lib/js/bootstrap.min.js"></script>
</body>
</html>
