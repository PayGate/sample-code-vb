@*
    File showing full set of inputs that it is possible to make using Web Payment Request
    Same functionality, simple format
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
            lastResponse = respTxtXml

            Dim r = TryCast(response.SinglePaymentResponse.Item, PayHost.WebPaymentResponseType)
            Dim t = r

            If r.Status.StatusName.ToString() = "ValidationError" Then
                lastResponse = r.Status.StatusDetail
            ElseIf r.Status.StatusName.ToString() = "WebRedirectRequired" Then
                'Redirect is required
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
                redirectHtml = Html.Raw(redirectText)
            End If

            afterAuthHtml = Html.Raw(PayhostSOAP.htmlAfterAuth(lastRequest, respTxtXml, redirectText))
        Catch e As Exception
            Dim err = e.Message
        End Try
    End If

End Code

<html>
<head>
    <title>PayHost - Initiate</title>
    <style type="text/css">
        label {
            margin-top: 5px;
            display: inline-block;
            width: 150px;
        }
    </style>
</head>
<body>
    <a href="../../../PayHost/singlePayment/webPayment/index.vbhtml">back to Initiate</a><br>
    <form action="simple_initiate.vbhtml" method="post">
        <label for="pgid">PayGate ID</label>
        <input type="text" name="pgid" id="pgid" value="@pgid" />
        <br>
        <label for="reference">Reference</label>
        <input type="text" name="reference" id="reference" value="@reference" />
        <br>
        <label for="amount">Amount</label>
        <input type="text" name="amount" id="amount" value="@amount" />
        <br>
        <label for="currency">Currency</label>
        <input type="text" name="currency" id="currency" value="@currency" />
        <br>
        <label for="transDate">Transaction Date</label>
        <input type="text" name="transDate" id="transDate" value="@DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")" />
        <br>
        <label for="locale">Locale</label>
        <input type="text" name="locale" id="locale" value="@locale" />
        <br>
        <label for="encryptionKey">Encryption Key</label>
        <input type="text" name="encryptionKey" id="encryptionKey" value="@encryptionKey" />
        <br>
        <h3>Paymethod and User Fields</h3>
        <label for="payMethod">Pay Method</label>
        <input type="text" name="payMethod" id="payMethod" value="" placeholder="optional" />
        <br>
        <label for="payMethodDetail">Pay Method Detail</label>
        <input type="text" name="payMethodDetail" id="payMethodDetail" value="" placeholder="optional" />
        <br>
        <div class="userDefined">
            <label for="userFields">User Defined</label>
            <input type="text" name="userKey1" id="userKey1" class="userKey" value="" placeholder="Key" />
            <input type="text" name="userField1" id="userField1" class="userField" value="" placeholder="Value" />
        </div>


        <span id="fieldHolder"></span>
        <br>
        <button id="addUserFieldBtn" type="button">Add User Defined Fields</button>
        <br>
        <h3>Customer Details</h3>
        <label for="customerTitle">Title</label>
        <input type="text" name="customerTitle" id="customerTitle" value="@customerTitle" placeholder="optional" />
        <br>
        <label for="firstName">First Name</label>
        <input type="text" name="firstName" id="firstName" value="@firstName" placeholder="required" />
        <br>
        <label for="middleName">Middle Name</label>
        <input type="text" name="middleName" id="middleName" value="" placeholder="optional" />
        <br>
        <label for="lastName">Last Name</label>
        <input type="text" name="lastName" id="lastName" value="@lastName" placeholder="required" />
        <br>
        <label for="telephone">Telephone</label>
        <input type="text" name="telephone" id="telephone" value="" placeholder="optional" />
        <br>
        <label for="mobile">Mobile</label>
        <input type="text" name="mobile" id="mobile" value="" placeholder="optional" />
        <br>
        <label for="fax">Fax</label>
        <input type="text" name="fax" id="fax" value="" placeholder="optional" />
        <br>
        <label for="email">Email</label>
        <input type="text" name="email" id="email" value="@email" placeholder="required" />
        <br>
        <label for="dateOfBirth">Date Of Birth</label>
        <input name="dateOfBirth" id="dateOfBirth" value="" placeholder="YYYY-MM-DD optional" />
        <br>
        <label for="socialSecurity">Social Security</label>
        <input name="socialSecurity" id="socialSecurity" value="" placeholder="optional" />
        <br>
        <h3>Risk Fields</h3>
        <label for="riskAccNum">Account Number</label>
        <input type="text" name="riskAccNum" id="riskAccNum" value="" placeholder="optional" />
        <br>
        <label for="riskIpAddr">Ip Address</label>
        <input type="text" name="riskIpAddr" id="riskIpAddr" value="" placeholder="optional" />
        <br>
        <h3>Shipping Fields</h3>
        <label for="deliveryDate">Delivery Date</label>
        <input type="text" name="deliveryDate" id="deliveryDate" value="" placeholder="YYYY-MM-DD optional" />
        <br>
        <label for="deliveryMethod">Delivery Method</label>
        <input type="text" name="deliveryMethod" id="deliveryMethod" value="" placeholder="optional" />
        <br>
        <label for="installRequired">Installation Required</label>
        <input type="checkbox" name="installRequired" id="installRequired" value="true" />
        <br>
        <h3>Address Fields</h3>
        <label for="incCustomer">Customer</label>
        <input name="incCustomer" id="incCustomer" type="checkbox" value="incCustomer" checked />
        <br>
        <label for="incBilling">Billing</label>
        <input name="incBilling" id="incBilling" type="checkbox" value="incBilling" checked />
        <br>
        <label for="incShipping">Shipping</label>
        <input name="incShipping" id="incShipping" type="checkbox" value="incShipping" />
        <br>
        <label for="addressLine1">Address Line 1</label>
        <input type="text" name="addressLine1" id="addressLine1" value="" placeholder="optional" />
        <br>
        <label for="addressLine2">Address Line 2</label>
        <input type="text" name="addressLine2" id="addressLine2" value="" placeholder="optional" />
        <br>
        <label for="addressLine3">Address Line 3</label>
        <input type="text" name="addressLine3" id="addressLine3" value="" placeholder="optional" />
        <br>
        <label for="zip">Zip</label>
        <input type="text" name="zip" id="zip" value="" placeholder="optional" />
        <br>
        <label for="city">City</label>
        <input type="text" name="city" id="city" value="" placeholder="optional" />
        <br>
        <label for="state">State</label>
        <input type="text" name="state" id="state" value="" placeholder="optional" />
        <br>
        <label for="country">Country</label>
        <select name="country" id="country">
            @countrySelection
        </select>
        <br>
        <h3>Redirect Fields</h3>
        <label for="retUrl">Return URL</label>
        <input type="text" name="retUrl" id="retUrl" value="@returnPath" />
        <br>
        <label for="notifyURL">Notify URL</label>
        <input type="text" name="notifyURL" id="notifyURL" value="@notifyPath" />
        <br>
        <label for="target">Target</label>
        <input type="text" name="target" id="target" value="" />
        <br>
        <h3>Airline Fields</h3>
        <label for="ticketNumber">Ticket Number</label>
        <input type="text" name="ticketNumber" id="ticketNumber" value="" />
        <br>
        <label for="PNR">PNR</label>
        <input type="text" name="PNR" id="PNR" value="" />
        <br>
        <h4>Passenger</h4>
        <label for="travellerType">Traveller Type</label>
        <select name="travellerType" id="travellerType">
            <option value="A">Adult</option>
            <option value="C">Child</option>
            <option value="T">Teenager</option>
            <option value="I">Infant</option>
        </select>
        <br>
        <h4>Flight Details</h4>
        <label for="departureAirport">Departure Airport</label>
        <input type="text" name="departureAirport" id="departureAirport" value="" placeholder="eg:ABC" />
        <br>
        <label for="departureCountry">Departure Country</label>
        <input type="text" name="departureCountry" id="departureCountry" value="" placeholder="eg:ABC" />
        <br>
        <label for="departureCity">Departure City</label>
        <input type="text" name="departureCity" id="departureCity" value="" placeholder="eg:ABC" />
        <br>
        <label for="departureDateTime">Departure Date & Time</label>
        <input type="text" name="departureDateTime" id="departureDateTime" value="" placeholder="eg:2015-01-01T12:00:00" />
        <br>
        <label for="arrivalAirport">Arrival Airport</label>
        <input type="text" name="arrivalAirport" id="arrivalAirport" value="" placeholder="eg:ABC" />
        <br>
        <label for="arrivalCountry">Arrival Country</label>
        <input type="text" name="arrivalCountry" id="arrivalCountry" value="" placeholder="eg:ABC" />
        <br>
        <label for="arrivalCity">Arrival City</label>
        <input type="text" name="arrivalCity" id="arrivalCity" value="" placeholder="eg:ABC" />
        <br>
        <label for="arrivalDateTime">Arrival Date & Time</label>
        <input type="text" name="arrivalDateTime" id="arrivalDateTime" value="" placeholder="eg:2015-01-01T12:00:00" />
        <br>
        <label for="marketingCarrierCode">Marketing Carrier Code</label>
        <input type="text" name="marketingCarrierCode" id="marketingCarrierCode" value="" placeholder="Z1" />
        <br>
        <label for="marketingCarrierName">Marketing Carrier Name</label>
        <input type="text" name="marketingCarrierName" id="marketingCarrierName" value="" />
        <br>
        <label for="issuingCarrierCode">Issuing Carrier Code</label>
        <input type="text" name="issuingCarrierCode" id="issuingCarrierCode" value="" placeholder="YY" />
        <br>
        <label for="issuingCarrierName">Issuing Carrier Name</label>
        <input type="text" name="issuingCarrierName" id="issuingCarrierName" value="" />
        <br>
        <label for="flightNumber">Flight Number</label>
        <input type="text" name="flightNumber" id="flightNumber" value="" />
        <br>
        <input id="doAuthBtn" type="submit" name="btnSubmit" value="Do Auth" />
        <br>
    </form>
    <label for="request" style="vertical-align: top;">REQUEST:</label>
    <textarea rows="20" cols="100" id="request">@lastRequest</textarea>
    <br>

    <label for="response" style="vertical-align: top;">RESPONSE:</label>
    <textarea rows="20" cols="100" id="response">@lastResponse</textarea>
    <br>

    <label style="vertical-align: top;" for="redirect">REDIRECT:</label>
    @redirectHtml
    <br>


    <script type="text/javascript" src="../../../lib/js/jquery-1.10.2.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#addUserFieldBtn').on('click', function () {

                var fieldHolder = $('#fieldHolder'),
                    lastUserFieldDiv = fieldHolder.prev('.userDefined'),
                    key = lastUserFieldDiv.find('.userKey'),
                    i = parseInt(key.attr('id').replace('userKey', ''));

                i++;

                var newUserFieldsDiv = '<div class="userDefined">' +
                    '    <label for="reference">User Defined</label>' +
                    '    <input class="userKey" type="text" name="userKey' + i + '" id="userKey' + i + '" value="" placeholder="Key" />' +
                    '    <input class="userField" type="text" name="userField' + i + '" id="userField' + i + '" value="" placeholder="Value" />' +
                    '</div>';

                fieldHolder.before(newUserFieldsDiv);
            });
        });
    </script>
</body>
</html>
