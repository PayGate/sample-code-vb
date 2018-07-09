'
' Copyright (c) 2018 PayGate (Pty) Ltd
'
' Author: App Inlet (Pty) Ltd
'
' Released under the GNU General Public License
'
Imports System
Imports System.Text
Imports System.Globalization
Imports System.Web
Imports System.Xml.Serialization
Imports System.IO

Public Module PayhostSOAP
    Public DEFAULT_PGID As String = "10011072130"
    Public DEFAULT_AMOUNT As Integer = 3299
    Public DEFAULT_CURRENCY As String = "ZAR"
    Public DEFAULT_LOCALE As String = "en-us"
    Public DEFAULT_ENCRYPTION_KEY As String = "test"
    Public DEFAULT_TITLE As String = "Mr"
    Public DEFAULT_FIRST_NAME As String = "PayGate"
    Public DEFAULT_LAST_NAME As String = "Test"
    Public DEFAULT_EMAIL As String = "itsupport@paygate.co.za"
    Public DEFAULT_COUNTRY As String = "ZAF"
    Public DEFAULT_NOTIFY_URL As String = "http://www.gatewaymanagementservices.com/ws/gotNotify.php"

    Function makeLookupVaultRequest(ByVal r As HttpRequestBase) As PayHost.LookUpVaultRequestType
        Dim request As PayHost.LookUpVaultRequestType = New PayHost.LookUpVaultRequestType()
        request.Account = New PayHost.PayGateAccountType()
        If r("payGateId") IsNot Nothing AndAlso r("payGateId") <> "" Then request.Account.PayGateId = r("payGateId")
        If r("encryptionKey") IsNot Nothing AndAlso r("encryptionKey") <> "" Then request.Account.Password = r("encryptionKey")
        If r("vaultId") IsNot Nothing AndAlso r("vaultId") <> "" Then request.VaultId = r("vaultId")
        Dim j As Integer = 0

        While r("userKey" & (j + 1)) IsNot Nothing AndAlso r("userField" & (j + 1)) IsNot Nothing
            j += 1
        End While

        If j > 0 Then
            request.UserDefinedFields = New PayHost.KeyValueType(j - 1) {}

            For k As Integer = 0 To j - 1

                If r("userKey" & (k + 1)) IsNot Nothing AndAlso r("userField" & (k + 1)) IsNot Nothing Then
                    request.UserDefinedFields(k) = New PayHost.KeyValueType()
                    request.UserDefinedFields(k).key = r("userKey" & (k + 1))
                    request.UserDefinedFields(k).value = r("userField" & (k + 1))
                End If
            Next
        End If

        Return request
    End Function

    Function makeDeleteVaultRequest(ByVal r As HttpRequestBase) As PayHost.DeleteVaultRequestType
        Dim request As PayHost.DeleteVaultRequestType = New PayHost.DeleteVaultRequestType()
        request.Account = New PayHost.PayGateAccountType()
        If r("payGateId") IsNot Nothing AndAlso r("payGateId") <> "" Then request.Account.PayGateId = r("payGateId")
        If r("encryptionKey") IsNot Nothing AndAlso r("encryptionKey") <> "" Then request.Account.Password = r("encryptionKey")
        If r("vaultId") IsNot Nothing AndAlso r("vaultId") <> "" Then request.VaultId = r("vaultId")
        Dim j As Integer = 0

        While r("userKey" & (j + 1)) IsNot Nothing AndAlso r("userField" & (j + 1)) IsNot Nothing
            j += 1
        End While

        If j > 0 Then
            request.UserDefinedFields = New PayHost.KeyValueType(j - 1) {}

            For k As Integer = 0 To j - 1

                If r("userKey" & (k + 1)) IsNot Nothing AndAlso r("userField" & (k + 1)) IsNot Nothing Then
                    request.UserDefinedFields(k) = New PayHost.KeyValueType()
                    request.UserDefinedFields(k).key = r("userKey" & (k + 1))
                    request.UserDefinedFields(k).value = r("userField" & (k + 1))
                End If
            Next
        End If

        Return request
    End Function

    Function makeCardVaultRequest(ByVal r As HttpRequestBase) As PayHost.CardVaultRequestType
        Dim request As PayHost.CardVaultRequestType = New PayHost.CardVaultRequestType()
        request.Account = New PayHost.PayGateAccountType()
        If r("payGateId") IsNot Nothing AndAlso r("payGateId") <> "" Then request.Account.PayGateId = r("payGateId")
        If r("encryptionKey") IsNot Nothing AndAlso r("encryptionKey") <> "" Then request.Account.Password = r("encryptionKey")
        If r("cardNumber") IsNot Nothing AndAlso r("cardNumber") <> "" Then request.CardNumber = r("cardNumber")
        If r("expiryDate") IsNot Nothing AndAlso r("expiryDate") <> "" Then request.CardExpiryDate = r("expiryDate")
        Dim j As Integer = 0

        While r("userKey" & (j + 1)) IsNot Nothing AndAlso r("userField" & (j + 1)) IsNot Nothing
            j += 1
        End While

        If j > 0 Then
            request.UserDefinedFields = New PayHost.KeyValueType(j - 1) {}

            For k As Integer = 0 To j - 1

                If r("userKey" & (k + 1)) IsNot Nothing AndAlso r("userField" & (k + 1)) IsNot Nothing Then
                    request.UserDefinedFields(k) = New PayHost.KeyValueType()
                    request.UserDefinedFields(k).key = r("userKey" & (k + 1))
                    request.UserDefinedFields(k).value = r("userField" & (k + 1))
                End If
            Next
        End If

        Return request
    End Function

    Function makeWebPaymentRequest(ByVal r As HttpRequestBase) As PayHost.WebPaymentRequestType
        Dim request As PayHost.WebPaymentRequestType = New PayHost.WebPaymentRequestType()
        Dim amount As String = ""
        request.Account = New PayHost.PayGateAccountType()
        If r("pgid") IsNot Nothing AndAlso r("pgid") <> "" Then request.Account.PayGateId = r("pgid")
        SessionModel.pgid = request.Account.PayGateId
        If r("encryptionKey") IsNot Nothing AndAlso r("encryptionKey") <> "" Then request.Account.Password = r("encryptionKey")
        SessionModel.key = request.Account.Password
        request.Customer = New PayHost.PersonType()
        If r("customerTitle") IsNot Nothing AndAlso r("customerTitle") <> "" Then request.Customer.Title = r("customerTitle")
        If r("firstName") IsNot Nothing AndAlso r("firstName") <> "" Then request.Customer.FirstName = r("firstName")
        If r("middleName") IsNot Nothing AndAlso r("middleName") <> "" Then request.Customer.MiddleName = r("middleName")
        If r("lastName") IsNot Nothing AndAlso r("lastName") <> "" Then request.Customer.LastName = r("lastName")

        If r("telephone") IsNot Nothing AndAlso r("telephone") <> "" Then
            request.Customer.Telephone = New String(0) {}
            request.Customer.Telephone(0) = r("telephone")
        End If

        If r("mobile") IsNot Nothing AndAlso r("mobile") <> "" Then
            request.Customer.Mobile = New String(0) {}
            request.Customer.Mobile(0) = r("mobile")
        End If

        If r("fax") IsNot Nothing AndAlso r("fax") <> "" Then
            request.Customer.Fax = New String(0) {}
            request.Customer.Fax(0) = r("fax")
        End If

        If r("email") IsNot Nothing AndAlso r("email") <> "" Then
            request.Customer.Email = New String(0) {}
            request.Customer.Email(0) = r("email")
        End If

        If r("dateOfBirth") IsNot Nothing AndAlso r("dateOfBirth") <> "" Then
            request.Customer.DateOfBirth = DateTime.Parse(r("dateOfBirth"))
        End If

        If r("nationality") IsNot Nothing AndAlso r("nationality") <> "" Then request.Customer.Nationality = r("nationality")
        If r("idNumber") IsNot Nothing AndAlso r("idNumber") <> "" Then request.Customer.IdNumber = r("idNumber")
        If r("idType") IsNot Nothing AndAlso r("idType") <> "" Then request.Customer.IdType = r("idType")
        If r("socialSecurity") IsNot Nothing AndAlso r("socialSecurity") <> "" Then request.Customer.SocialSecurityNumber = r("socialSecurity")

        If r("incCustomer") = "incCustomer" Then
            request.Customer.Address = New PayHost.AddressType()
            request.Customer.Address.AddressLine = New String(2) {}
            request.Customer.Address.AddressLine(0) = r("addressLine1")
            request.Customer.Address.AddressLine(1) = r("addressLine2")
            request.Customer.Address.AddressLine(2) = r("addressLine3")

            If r("city") IsNot Nothing AndAlso r("city") <> "" Then
                request.Customer.Address.City = r("city")
            End If

            If r("country") IsNot Nothing AndAlso r("country") <> "" Then
                request.Customer.Address.Country = New PayHost.CountryType()
                request.Customer.Address.Country = CType([Enum].Parse(GetType(PayHost.CountryType), r("country")), PayHost.CountryType)
                request.Customer.Address.CountrySpecified = True
            End If

            If r("state") IsNot Nothing AndAlso r("state") <> "" Then request.Customer.Address.State = r("state")
            If r("zip") IsNot Nothing AndAlso r("zip") <> "" Then request.Customer.Address.Zip = r("zip")
        End If

        request.PaymentType = New PayHost.PaymentType(0) {}
        request.PaymentType(0) = New PayHost.PaymentType()

        If r("payMethod") IsNot Nothing AndAlso r("payMethod") <> "" Then
            request.PaymentType(0).Method = New PayHost.PaymentMethodType()
            request.PaymentType(0).Method = CType([Enum].Parse(GetType(PayHost.PaymentMethodType), r("payMethod")), PayHost.PaymentMethodType)
        End If

        If r("payMethodDetail") IsNot Nothing AndAlso r("payMethodDetail") <> "" Then
            request.PaymentType(0).Detail = r("payMethodDetail")
        End If

        request.Redirect = New PayHost.RedirectRequestType()
        If r("retUrl") IsNot Nothing AndAlso r("retUrl") <> "" Then request.Redirect.ReturnUrl = r("retUrl")
        If r("notifyUrl") IsNot Nothing AndAlso r("notifyUrl") <> "" Then request.Redirect.NotifyUrl = r("notifyUrl")
        request.Order = New PayHost.OrderType()
        If r("reference") IsNot Nothing AndAlso r("reference") <> "" Then request.Order.MerchantOrderId = r("reference")
        SessionModel.reference = request.Order.MerchantOrderId

        If r("currency") IsNot Nothing AndAlso r("currency") <> "" Then
            request.Order.Currency = CType([Enum].Parse(GetType(PayHost.CurrencyType), r("currency")), PayHost.CurrencyType)
        End If

        If r("amount") IsNot Nothing AndAlso r("amount") <> "" Then
            'Replace decimal comma with decimal point 
            amount = r("amount").Replace(",", ".")
            'And convert to cents
            request.Order.Amount = CType(100 * Decimal.Parse(r("amount"), CultureInfo.InvariantCulture), Integer)
        End If
        If r("transDate") IsNot Nothing AndAlso r("transDate") <> "" Then request.Order.TransactionDate = DateTime.ParseExact(r("transDate"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)

        If r("incBilling") = "incBilling" Then
            request.Order.BillingDetails = New PayHost.BillingDetailsType()
            request.Order.BillingDetails.Customer = request.Customer
            request.Order.BillingDetails.Address = New PayHost.AddressType()
            request.Order.BillingDetails.Address.AddressLine = New String(2) {}
            request.Order.BillingDetails.Address.AddressLine(0) = r("addressLine1")
            request.Order.BillingDetails.Address.AddressLine(1) = r("addressLine2")
            request.Order.BillingDetails.Address.AddressLine(2) = r("addressLine3")

            If r("city") IsNot Nothing AndAlso r("city") <> "" Then
                request.Order.BillingDetails.Address.City = r("city")
            End If

            If r("country") IsNot Nothing AndAlso r("country") <> "" Then
                request.Order.BillingDetails.Address.Country = New PayHost.CountryType()
                request.Order.BillingDetails.Address.Country = CType([Enum].Parse(GetType(PayHost.CountryType), r("country")), PayHost.CountryType)
                request.Order.BillingDetails.Address.CountrySpecified = True
            End If

            If r("state") IsNot Nothing AndAlso r("state") <> "" Then request.Order.BillingDetails.Address.State = r("state")
            If r("zip") IsNot Nothing AndAlso r("zip") <> "" Then request.Order.BillingDetails.Address.Zip = r("zip")
        End If

        If r("incShipping") = "incShipping" Then
            request.Order.ShippingDetails = New PayHost.ShippingDetailsType()
            request.Order.ShippingDetails.Customer = request.Order.BillingDetails.Customer
            request.Order.ShippingDetails.Address = request.Order.BillingDetails.Address

            If r("deliveryDate") IsNot Nothing AndAlso r("deliveryDate") <> "" Then
                request.Order.ShippingDetails.DeliveryDate = DateTime.Parse(r("deliveryDate"))
                request.Order.ShippingDetails.DeliveryDateSpecified = True
            End If

            If r("deliveryMethod") IsNot Nothing AndAlso r("deliveryMethod") <> "" Then
                request.Order.ShippingDetails.DeliveryMethod = r("deliveryMethod")
            End If

            If r("installRequired") = "installRequired" Then
                request.Order.ShippingDetails.InstallationRequested = True
                request.Order.ShippingDetails.InstallationRequestedSpecified = True
            End If
        End If

        If r("PNR") IsNot Nothing AndAlso r("PNR") <> "" Then
            request.Order.AirlineBookingDetails = New PayHost.AirlineBookingDetailsType()
            request.Order.AirlineBookingDetails.PNR = r("PNR")
            If r("ticketNumber") IsNot Nothing AndAlso r("ticketNumber") <> "" Then request.Order.AirlineBookingDetails.TicketNumber = r("ticketNumber")
            request.Order.AirlineBookingDetails.Passengers = New PayHost.PassengerType(0) {}
            request.Order.AirlineBookingDetails.Passengers(0) = New PayHost.PassengerType()

            If r("travellerType") IsNot Nothing AndAlso r("travellerType") <> "" Then
                request.Order.AirlineBookingDetails.Passengers(0).TravellerType = CType([Enum].Parse(GetType(PayHost.PassengerTypeTravellerType), r("travellerType")), PayHost.PassengerTypeTravellerType)
            End If

            request.Order.AirlineBookingDetails.Passengers(0).Passenger = request.Order.BillingDetails.Customer
            request.Order.AirlineBookingDetails.FlightLegs = New PayHost.FlightLegType(0) {}
            request.Order.AirlineBookingDetails.FlightLegs(0) = New PayHost.FlightLegType()
            If r("departureAirport") IsNot Nothing AndAlso r("departureAirport") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).DepartureAirport = r("departureAirport")
            If r("departureCountry") IsNot Nothing AndAlso r("departureCountry") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).DepartureCountry = CType([Enum].Parse(GetType(PayHost.CountryType), r("departureCountry")), PayHost.CountryType)
            If r("departureCity") IsNot Nothing AndAlso r("departureCity") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).DepartureCity = r("departureCity")
            If r("departureCountry") IsNot Nothing AndAlso r("departureCountry") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).DepartureCountry = CType([Enum].Parse(GetType(PayHost.CountryType), r("departureCountry")), PayHost.CountryType)
            If r("departureDateTime") IsNot Nothing AndAlso r("departureDateTime") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).DepartureDateTime = DateTime.Parse(r("departureDateTime"))
            If r("arrivalAirport") IsNot Nothing AndAlso r("arrivalAirport") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).ArrivalAirport = r("arrivalAirport")
            If r("arrivalCountry") IsNot Nothing AndAlso r("arrivalCountry") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).ArrivalCountry = CType([Enum].Parse(GetType(PayHost.CountryType), r("arrivalCountry")), PayHost.CountryType)
            If r("arrivalCity") IsNot Nothing AndAlso r("arrivalCity") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).ArrivalCity = r("arrivalCity")
            If r("arrivalCountry") IsNot Nothing AndAlso r("arrivalCountry") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).ArrivalCountry = CType([Enum].Parse(GetType(PayHost.CountryType), r("arrivalCountry")), PayHost.CountryType)
            If r("arrivalDateTime") IsNot Nothing AndAlso r("arrivalDateTime") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).ArrivalDateTime = DateTime.Parse(r("arrivalDateTime"))
            If r("marketingCarrierCode") IsNot Nothing AndAlso r("marketingCarrierCode") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).MarketingCarrierCode = r("marketingCarrierCode")
            If r("marketingCarrierName") IsNot Nothing AndAlso r("marketingCarrierName") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).MarketingCarrierName = r("marketingCarrierName")
            If r("issuingCarrierCode") IsNot Nothing AndAlso r("issuingCarrierCode") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).IssuingCarrierCode = r("issuingCarrierCode")
            If r("issuingCarrierName") IsNot Nothing AndAlso r("issuingCarrierName") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).IssuingCarrierName = r("issuingCarrierName")
            If r("flightNumber") IsNot Nothing AndAlso r("flightNumber") <> "" Then request.Order.AirlineBookingDetails.FlightLegs(0).FlightNumber = r("flightNumber")
        End If

        If r("riskAccNum") IsNot Nothing AndAlso r("riskAccNum") <> "" Then
            request.Risk = New PayHost.RiskType()
            request.Risk.AccountNumber = r("riskAccNum")

            If r("riskIpAddr") IsNot Nothing AndAlso r("riskIpAddr") <> "" Then
                request.Risk.IpV4Address = r("riskIpAddr")
            End If
        End If

        Dim j As Integer = 0

        While r("userKey" & (j + 1)) IsNot Nothing AndAlso r("userField" & (j + 1)) IsNot Nothing
            j += 1
        End While

        If j > 0 Then
            request.UserDefinedFields = New PayHost.KeyValueType(j - 1) {}

            For k As Integer = 0 To j - 1

                If r("userKey" & (k + 1)) IsNot Nothing AndAlso r("userField" & (k + 1)) IsNot Nothing Then
                    request.UserDefinedFields(k) = New PayHost.KeyValueType()
                    request.UserDefinedFields(k).key = r("userKey" & (k + 1))
                    request.UserDefinedFields(k).value = r("userField" & (k + 1))
                End If
            Next
        End If

        Return request
    End Function

    Function getXMLText(ByVal requestObject As Object) As String
        Dim xmlSerializer As XmlSerializer = New XmlSerializer(requestObject.[GetType]())

        Using s As MemoryStream = New MemoryStream()
            xmlSerializer.Serialize(s, requestObject)
            Dim byteArray = s.GetBuffer()
            Dim str = New StringBuilder()

            For i = 0 To byteArray.Length - 1
                str.Append(ChrW(byteArray(i)))
            Next

            Return str.ToString()
        End Using
    End Function

    Function htmlAfterAuth(ByVal lastRequest As String, ByVal responseText As String, ByVal redirectText As String) As String
        Dim html = "<div class=""row"" style=""margin-bottom: 15px; "">
                   <div class=""col-sm-offset-4 col-sm-4"">
                    <button id = ""showRequestBtn"" class=""btn btn-primary btn-block"" type=""button"" data-toggle=""collapse"" data-target=""#requestDiv"" aria-expanded=""false"" aria-controls=""requestDiv"">
                        Request
                    </button>
                </div>
            </div>
            <div id = ""requestDiv"" class=""row collapse well well-sm"">
                <textarea rows = ""20"" cols=""100"" id=""request"" class=""form-control"">" & lastRequest
        html += "</textarea>
            </div>
            <div class=""row"" style=""margin-bottom: 15px; "">
                   <div class=""col-sm-offset-4 col-sm-4"">
                    <button id = ""showResponseBtn"" class=""btn btn-primary btn-block"" type=""button"" data-toggle=""collapse"" data-target=""#responseDiv"" aria-expanded=""false"" aria-controls=""responseDiv"">
                        Response
                    </button>
                </div>
            </div>
            <div id = ""responseDiv"" class=""row collapse well well-sm"">
                <textarea rows = ""20"" cols=""100"" id=""response"" class=""form-control"">" & responseText
        html += "</textarea>
            </div>" & redirectText
        Return html
    End Function
End Module
