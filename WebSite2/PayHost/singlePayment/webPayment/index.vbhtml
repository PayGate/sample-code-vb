﻿@*
    File showing full set of inputs that it is possible to make using Web Payment Request
*@

@Imports Sytem.Text
@Imports System.Web
@Code
    'Manadtory key variables
    Dim pgid = If(SessionModel.pgid IsNot Nothing, SessionModel.pgid, PayhostSOAP.DEFAULT_PGID)
    Dim reference = If(SessionModel.reference IsNot Nothing, SessionModel.reference, GlobalUtility.generateReference())
    Dim encryptionKey = PayhostSOAP.DEFAULT_ENCRYPTION_KEY
    Dim amount = PayhostSOAP.DEFAULT_AMOUNT
    Dim currency = PayhostSOAP.DEFAULT_CURRENCY
    Dim locale = PayhostSOAP.DEFAULT_LOCALE
    Dim customerTitle = PayhostSOAP.DEFAULT_TITLE
    Dim firstName = PayhostSOAP.DEFAULT_FIRST_NAME
    Dim lastName = PayhostSOAP.DEFAULT_LAST_NAME
    Dim email = PayhostSOAP.DEFAULT_EMAIL

    'Razor fields
    Dim root = GlobalUtility.getUriParts()
    Dim returnPath As String = root(0) & "//" + root(2) & "/Payhost/result.vbhtml"
    Dim notifyPath As String = root(0) & "//" + root(2) & "/Payhost/notify.vbhtml"
    Dim redirectHtml = Html.Raw("")
    Dim afterAuthHtml = Html.Raw("")

    Dim lastRequest = "This was the last request"
    Dim lastResponse = "This was the last response"
    Dim response = New PayHost.SinglePaymentResponse1()

    'Create the PayHost client
    Dim payHostt As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

    'Save for the session
    SessionModel.pgid = pgid
    SessionModel.reference = reference
    SessionModel.key = encryptionKey

    Dim countrySelection = Html.Raw(GlobalUtility.generateCountrySelectOptions())

    'User defined fields
    Dim userFieldHtml = ""
    userFieldHtml += "<div class=""form-group userDefined"">"
    userFieldHtml += "<label for=""userFields"" class=""col-sm-3 control-label"">User Defined</label>"
    userFieldHtml += "<div class=""col-sm-4""><input class=""form-control userKey"" type=""text"" name=""userKey1"" id=""userKey1"" value="""" placeholder=""Key"" />"
    userFieldHtml += "</div><div class=""col-sm-4"">"
    userFieldHtml += "<input class=""form-control userField"" type=""text"" name=""userField1""id=""userField1"" value="""" placeholder=""Value"" />"
    userFieldHtml += "</div></div>"

    Dim userFieldHtmlRaw = Html.Raw(userFieldHtml)

    If Request("btnSubmit") IsNot Nothing Then
        Dim webPaymentRequest As PayHost.WebPaymentRequestType = PayhostSOAP.makeWebPaymentRequest(Request)
        lastRequest = PayhostSOAP.getXMLText(webPaymentRequest)
        Dim sprequest = New PayHost.SinglePaymentRequest With {
            .Item = webPaymentRequest
        }
        Dim sprequest1 = New PayHost.SinglePaymentRequest1(sprequest)

        Try
            Dim redirectText = ""
            response = payHostt.SinglePayment(sprequest1)
            Dim respTxtXml = PayhostSOAP.getXMLText(response.SinglePaymentResponse)
            Dim r = TryCast(response.SinglePaymentResponse.Item, PayHost.WebPaymentResponseType)
            Dim t = r

            If r.Status.StatusName.ToString() = "ValidationError" Then
                lastResponse = r.Status.StatusDetail
            ElseIf r.Status.StatusName.ToString() = "WebRedirectRequired" Then
                redirectText += "<div class=""row"" style=""margin-bottom: 15px; "">
                               <div class=""col-sm-offset-4 col-sm-4"">
                    <button id = ""showRedirectBtn"" class=""btn btn-primary btn-block"" type=""button"" data-toggle=""collapse"" data-target=""#redirectDiv"" aria-expanded=""false"" aria-controls=""redirectDiv"">
                        Redirect
                    </button>
                </div>
            </div>"
                redirectText += "<div id = ""redirectDiv"" class=""row collapse well well-sm"">
        <textarea rows = ""5"" cols=""100"" id=""redirect"" class=""form-control"">"

                For Each item In r.Redirect.UrlParams
                    redirectText += "<input type=""hidden"" name=""" & item.key & """ value=""" + item.value & """/>" + Environment.NewLine
                Next

                redirectText += "</textarea>
        <!-- form action passed back from WS -->
        <form action = " & r.Redirect.RedirectUrl & " method=""post"" class=""form-horizonatal text-left"" role=""form"" style=""margin-top: 15px;"">
    </div>"

                For Each item1 In r.Redirect.UrlParams
                    redirectText += "<input type=""hidden"" name=""" & item1.key & """ value=""" + item1.value & """/>"
                Next

                redirectText += "<br><div class=""form-group"">
                            <div class=""col-sm-offset-4 col-sm-4"">
                                <img src=""../../../lib/images/loader.gif"" alt=""Processing"" class=""initialHide"" id=""submitLoader"">
                                <input class=""btn btn-success btn-block"" type=""submit"" name=""submitBtn"" id=""doSubmitBtn"" value=""submit"" />
                            </div>
                            <br>
                        </form>"
            End If

            afterAuthHtml = Html.Raw(PayhostSOAP.htmlAfterAuth(lastRequest, respTxtXml, redirectText))
        Catch e As Exception
            Dim err = e.Message
        End Try
    End If

End Code

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayHost - Initiate</title>
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
                    <span style="color: #f4f4f4; font-size: 18px; line-height: 45px; margin-right: 10px;"><strong>PayHost Web Payment</strong></span>
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
        <div style="background-color:#80b946; text-align: center; margin-top: 51px; margin-bottom: 15px; padding: 4px;"><strong>Initiate WebPayment</strong></div>
        <div class="container">
            <form role="form" class="form-horizontal text-left" action="index.vbhtml" method="post">
                <div class="form-group">
                    <label for="pgid" class="col-sm-3 control-label">PayGate ID</label>
                    <div class="col-sm-4">
                        <input class="form-control" type="text" name="pgid" id="pgid" value="@pgid" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="reference" class="col-sm-3 control-label">Reference</label>
                    <div class="col-sm-6">
                        <input class="form-control" type="text" name="reference" id="reference" value="@reference" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="amount" class="col-sm-3 control-label">Amount</label>
                    <div class="col-sm-2">
                        <input class="form-control" type="text" name="amount" id="amount" value="@amount" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="currency" class="col-sm-3 control-label">Currency</label>
                    <div class="col-sm-2">
                        <input class="form-control" type="text" name="currency" id="currency" value="@currency" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="transDate" class="col-sm-3 control-label">Transaction Date</label>
                    <div class="col-sm-5">
                        <input class="form-control" type="text" name="transDate" id="transDate" value="@DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="locale" class="col-sm-3 control-label">Locale</label>
                    <div class="col-sm-3">
                        <input class="form-control" type="text" name="locale" id="locale" value="@locale" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="encryptionKey" class="col-sm-3 control-label">Encryption Key</label>
                    <div class="col-sm-5">
                        <input class="form-control" type="text" name="encryptionKey" id="encryptionKey" value="@encryptionKey" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <button class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#paymethodAndUserDiv" aria-expanded="false" aria-controls="paymethodAndUserDiv">
                            Paymethod and User Fields
                        </button>
                    </div>
                </div>
                <div id="paymethodAndUserDiv" class="collapse well well-sm">
                    <div class="form-group">
                        <label for="payMethod" class="col-sm-3 control-label">Pay Method</label>
                        <div class="col-sm-2">
                            <input class="form-control" type="text" name="payMethod" id="payMethod" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="payMethodDetail" class="col-sm-3 control-label">Pay Method Detail</label>
                        <div class="col-sm-6">
                            <input class="form-control" type="text" name="payMethodDetail" id="payMethodDetail" value="" placeholder="optional" />
                        </div>
                    </div>
                    @userFieldHtmlRaw
                    <span id="fieldHolder"></span>
                    <div class="form-group">
                        <div class="col-sm-offset-3 col-sm-4">
                            <button class="btn btn-primary" id="addUserFieldBtn" type="button"><i class="glyphicon glyphicon-plus"></i> Add User Defined Fields</button>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <button class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#customerDetailDiv" aria-expanded="false" aria-controls="customerDetailDiv">
                            Customer Details
                        </button>
                    </div>
                </div>
                <div id="customerDetailDiv" class="collapse well well-sm">
                    <div class="form-group">
                        <label for="customerTitle" class="col-sm-3 control-label">Title</label>
                        <div class="col-sm-3">
                            <input class="form-control" type="text" name="customerTitle" id="customerTitle" value="@customerTitle" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="firstName" class="col-sm-3 control-label">First Name</label>
                        <div class="col-sm-6">
                            <input class="form-control" type="text" name="firstName" id="firstName" value="@firstName" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="middleName" class="col-sm-3 control-label">Middle Name</label>
                        <div class="col-sm-6">
                            <input class="form-control" type="text" name="middleName" id="middleName" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="lastName" class="col-sm-3 control-label">Last Name</label>
                        <div class="col-sm-6">
                            <input class="form-control" type="text" name="lastName" id="lastName" value="@lastName" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="telephone" class="col-sm-3 control-label">Telephone</label>
                        <div class="col-sm-5">
                            <input class="form-control" type="text" name="telephone" id="telephone" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="mobile" class="col-sm-3 control-label">Mobile</label>
                        <div class="col-sm-5">
                            <input class="form-control" type="text" name="mobile" id="mobile" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="fax" class="col-sm-3 control-label">Fax</label>
                        <div class="col-sm-5">
                            <input class="form-control" type="text" name="fax" id="fax" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="email" class="col-sm-3 control-label">Email</label>
                        <div class="col-sm-6">
                            <input class="form-control" type="text" name="email" id="email" value="@email" placeholder="required" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="dateOfBirth" class="col-sm-3 control-label">Date Of Birth</label>
                        <div class="col-sm-5">
                            <input class="form-control" name="dateOfBirth" id="dateOfBirth" value="" placeholder="YYYY-MM-DD" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="socialSecurity" class="col-sm-3 control-label">Social Security</label>
                        <div class="col-sm-7">
                            <input class="form-control" name="socialSecurity" id="socialSecurity" value="" placeholder="optional" />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <button class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#riskDiv" aria-expanded="false" aria-controls="riskDiv">
                            Risk Fields
                        </button>
                    </div>
                </div>
                <div id="riskDiv" class="collapse well well-sm ">
                    <div class="form-group">
                        <label for="riskAccNum" class="col-sm-3 control-label">Account Number</label>
                        <div class="col-sm-9">
                            <input class="form-control" type="text" name="riskAccNum" id="riskAccNum" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="riskIpAddr" class="col-sm-3 control-label">Ip Address</label>
                        <div class="col-sm-6">
                            <input class="form-control" type="text" name="riskIpAddr" id="riskIpAddr" value="" placeholder="optional" />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <button class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#shippingDiv" aria-expanded="false" aria-controls="shippingDiv">
                            Shipping Fields
                        </button>
                    </div>
                </div>
                <div id="shippingDiv" class="collapse well well-sm ">
                    <div class="form-group">
                        <label for="deliveryDate" class="col-sm-3 control-label">Delivery Date</label>
                        <div class="col-sm-5">
                            <input class="form-control" type="text" name="deliveryDate" id="deliveryDate" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="deliveryMethod" class="col-sm-3 control-label">Delivery Method</label>
                        <div class="col-sm-9">
                            <input class="form-control" type="text" name="deliveryMethod" id="deliveryMethod" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-offset-3 col-sm-9">
                            <div class="checkbox">
                                <label>
                                    <input type="checkbox" name="installRequired" id="installRequired" value="true" /> Installation Required
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <button class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#addressDiv" aria-expanded="false" aria-controls="addressDiv">
                            Address Fields
                        </button>
                    </div>
                </div>
                <div id="addressDiv" class="collapse well well-sm ">
                    <div class="form-group">
                        <div class="col-sm-offset-3 col-sm-9">
                            <label class="checkbox-inline">
                                <input name="incCustomer" id="incCustomer" type="checkbox" checked value="incCustomer" /> Customer
                            </label>
                            <label class="checkbox-inline">
                                <input name="incBilling" id="incBilling" type="checkbox" checked value="incBilling" /> Billing
                            </label>
                            <label class="checkbox-inline">
                                <input name="incShipping" id="incShipping" type="checkbox" value="incShipping" /> Shipping
                            </label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="addressLine1" class="col-sm-3 control-label">Address Line 1</label>
                        <div class="col-sm-7">
                            <input class="form-control" type="text" name="addressLine1" id="addressLine1" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="addressLine2" class="col-sm-3 control-label">Address Line 2</label>
                        <div class="col-sm-7">
                            <input class="form-control" type="text" name="addressLine2" id="addressLine2" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="addressLine3" class="col-sm-3 control-label">Address Line 3</label>
                        <div class="col-sm-7">
                            <input class="form-control" type="text" name="addressLine3" id="addressLine3" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="zip" class="col-sm-3 control-label">Zip</label>
                        <div class="col-sm-4">
                            <input class="form-control" type="text" name="zip" id="zip" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="city" class="col-sm-3 control-label">City</label>
                        <div class="col-sm-6">
                            <input class="form-control" type="text" name="city" id="city" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="state" class="col-sm-3 control-label">State</label>
                        <div class="col-sm-6">
                            <input class="form-control" type="text" name="state" id="state" value="" placeholder="optional" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="country" class="col-sm-3 control-label">Country</label>
                        <div class="col-sm-6">
                            <select name="country" id="country" class="form-control">@countrySelection</select>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <button class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#redirectFieldsDiv" aria-expanded="false" aria-controls="redirectFieldsDiv">
                            Redirect Fields
                        </button>
                    </div>
                </div>
                <div id="redirectFieldsDiv" class="collapse well well-sm">
                    <div class="form-group">
                        <label for="retUrl" class="col-sm-3 control-label">Return URL</label>
                        <div class="col-sm-9">
                            <input class="form-control" type="text" name="retUrl" id="retUrl" value="@returnPath" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="notifyURL" class="col-sm-3 control-label">Notify URL</label>
                        <div class="col-sm-9">
                            <input class="form-control" type="text" name="notifyURL" id="notifyURL" value="@notifyPath" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="target" class="col-sm-3 control-label">Target</label>
                        <div class="col-sm-9">
                            <input class="form-control" type="text" name="target" id="target" value="" />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-4">
                        <button class="btn btn-primary btn-block" type="button" data-toggle="collapse" data-target="#airlineFieldsDiv" aria-expanded="false" aria-controls="airlineFieldsDiv">
                            Airline Fields
                        </button>
                    </div>
                </div>
                <div id="airlineFieldsDiv" class="collapse well well-sm">
                    <div class="form-group">
                        <label for="ticketNumber" class="col-sm-3 control-label">Ticket Number</label>
                        <div class="col-sm-5">
                            <input class="form-control" type="text" name="ticketNumber" id="ticketNumber" value="" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="PNR" class="col-sm-3 control-label">PNR</label>
                        <div class="col-sm-5">
                            <input class="form-control" type="text" name="PNR" id="PNR" value="" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-12 text-center"><strong>Passenger</strong></div>
                    </div>
                    <div class="form-group">
                        <label for="travellerType" class="col-sm-3 control-label">Traveller Type</label>
                        <div class="col-sm-5">
                            <select class="form-control" name="travellerType" id="travellerType">
                                <option value="A">Adult</option>
                                <option value="C">Child</option>
                                <option value="T">Teenager</option>
                                <option value="I">Infant</option>
                            </select>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-12 text-center"><strong>Flight Details</strong></div>
                    </div>
                    <div class="form-group">
                        <label for="departureAirport" class="col-sm-3 control-label">Departure Airport</label>
                        <div class="col-sm-2">
                            <input class="form-control" type="text" name="departureAirport" id="departureAirport" value="" placeholder="eg:ABC" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="departureCountry" class="col-sm-3 control-label">Departure Country</label>
                        <div class="col-sm-2">
                            <input class="form-control" type="text" name="departureCountry" id="departureCountry" value="" placeholder="eg:ABC" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="departureCity" class="col-sm-3 control-label">Departure City</label>
                        <div class="col-sm-2">
                            <input class="form-control" type="text" name="departureCity" id="departureCity" value="" placeholder="eg:ABC" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="departureDateTime" class="col-sm-3 control-label">Departure Date & Time</label>
                        <div class="col-sm-4">
                            <input class="form-control" type="text" name="departureDateTime" id="departureDateTime" value="" placeholder="eg:2015-01-01T12:00:00" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="arrivalAirport" class="col-sm-3 control-label">Arrival Airport</label>
                        <div class="col-sm-2">
                            <input class="form-control" type="text" name="arrivalAirport" id="arrivalAirport" value="" placeholder="eg:ABC" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="arrivalCountry" class="col-sm-3 control-label">Arrival Country</label>
                        <div class="col-sm-2">
                            <input class="form-control" type="text" name="arrivalCountry" id="arrivalCountry" value="" placeholder="eg:ABC" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="arrivalCity" class="col-sm-3 control-label">Arrival City</label>
                        <div class="col-sm-2">
                            <input class="form-control" type="text" name="arrivalCity" id="arrivalCity" value="" placeholder="eg:ABC" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="arrivalDateTime" class="col-sm-3 control-label">Arrival Date & Time</label>
                        <div class="col-sm-4">
                            <input class="form-control" type="text" name="arrivalDateTime" id="arrivalDateTime" value="" placeholder="eg:2015-01-01T12:00:00" />
                        </div>
                    </div>
                    <br>
                    <div class="form-group">
                        <label for="marketingCarrierCode" class="col-sm-3 control-label">Marketing Carrier Code</label>
                        <div class="col-sm-4">
                            <input class="form-control" type="text" name="marketingCarrierCode" id="marketingCarrierCode" value="" placeholder="Z1" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="marketingCarrierName" class="col-sm-3 control-label">Marketing Carrier Name</label>
                        <div class="col-sm-4">
                            <input class="form-control" type="text" name="marketingCarrierName" id="marketingCarrierName" value="" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="issuingCarrierCode" class="col-sm-3 control-label">Issuing Carrier Code</label>
                        <div class="col-sm-4">
                            <input class="form-control" type="text" name="issuingCarrierCode" id="issuingCarrierCode" value="" placeholder="YY" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="issuingCarrierName" class="col-sm-3 control-label">Issuing Carrier Name</label>
                        <div class="col-sm-4">
                            <input class="form-control" type="text" name="issuingCarrierName" id="issuingCarrierName" value="" />
                        </div>
                    </div>
                    <br>
                    <div class="form-group">
                        <label for="flightNumber" class="col-sm-3 control-label">Flight Number</label>
                        <div class="col-sm-2">
                            <input class="form-control" type="text" name="flightNumber" id="flightNumber" value="" />
                        </div>
                    </div>
                </div>
                <br>
                <div class="form-group">
                    <div class=" col-sm-offset-4 col-sm-4">
                        <img src="../../../lib/images/loader.gif" alt="Processing" class="initialHide" id="authLoader">
                        <input class="btn btn-success btn-block" id="doAuthBtn" type="submit" name="btnSubmit" value="Do Auth" />
                    </div>
                </div>
                <br>
            </form>
            @afterAuthHtml
        </div>
    </div>
    <script type="text/javascript" src="../../../lib/js/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="../../../lib/js/bootstrap.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#doAuthBtn').on('click', function () {
                $(this).hide();
                $('#authLoader').show();
            });

            $('#doSubmitBtn').on('click', function () {
                $(this).hide();
                $('#submitLoader').show();
            });

            $('#addUserFieldBtn').on('click', function () {

                var lastUserFieldDiv = $('#fieldHolder').prev('.userDefined');

                var key = lastUserFieldDiv.find('.userKey');

                var i = parseInt(key.attr('id').replace('userKey', ''));
                i++;

                var newUserFieldsDiv = '<div class="form-group userDefined">' +
                    '    <label for="reference" class="col-sm-3 control-label">User Defined</label>' +
                    '    <div class="col-sm-4">' +
                    '        <input class="form-control userKey" type="text" name="userKey' + i + '" id="userKey' + i + '" value="" placeholder="Key" />' +
                    '    </div>' +
                    '    <div class="col-sm-4">' +
                    '        <input class="form-control userField" type="text" name="userField' + i + '" id="userField' + i + '" value="" placeholder="Value" />' +
                    '    </div>' +
                    '</div>';

                $('#fieldHolder').before(newUserFieldsDiv);
            });
        });
    </script>
</body>
</html>
