'
' Copyright (c) 2018 PayGate (Pty) Ltd
'
' Author: App Inlet (Pty) Ltd
'
' Released under the GNU General Public License
'
Imports Microsoft.VisualBasic
Imports System
Imports System.Net
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Web

' Class that provides initiate and query functionality for PayGate PayWeb3 applications
Public Class PaygatePayweb3
    'The url of the PayGate PayWeb 3 initiate page
    Public Shared initiate_url As String = "https://secure.paygate.co.za/payweb3/initiate.trans"

    'The url of the PayGate PayWeb 3 process page
    Public Shared process_url As String = "https://secure.paygate.co.za/payweb3/process.trans"

    'The url of the PayGate PayWeb 3 query page
    Public Shared query_url As String = "https://secure.paygate.co.za/payweb3/query.trans"

    'Dictionary contains the data to be posted to PayGate PayWeb 3 initiate
    Private initiateRequest As Dictionary(Of String, String) = New Dictionary(Of String, String)()

    'Dictionary contains the response data from the initiate
    Public initiateResponse As Dictionary(Of String, String) = New Dictionary(Of String, String)()

    'Dictionary contains the data returned from the initiate, required for the redirect of the client
    Public processRequest As Dictionary(Of String, String) = New Dictionary(Of String, String)()

    'Dictionary contains the data to be posted to PayGate PayWeb 3 query service
    Private queryRequest As Dictionary(Of String, String) = New Dictionary(Of String, String)()

    'Dictionary contains the response data from the query
    Public queryResponse As Dictionary(Of String, String)

    '* String
    '*
    '* Most common errors returned will be:
    '*
    '* DATA_CHK    -> Checksum posted does Not match the one calculated by PayGate, either due to an incorrect encryption key used Or a field that has been excluded from the checksum calculation
    '* DATA_PW     -> Mandatory fields have been excluded from the post to PayGate, refer to page 9 of the documentation as to what fields should be posted.
    '* DATA_CUR    -> The currency that has been posted to PayGate Is Not supported.
    '* PGID_NOT_EN -> The PayGate ID being used to post data to PayGate has Not yet been enabled, Or there are no payment methods setup on it.
    '*
    Public lastError As String

    Private transactionStatusArray As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)() From {
        {1, "Approved"},
        {2, "Declined"},
        {4, "Cancelled"}
    }

    'Private fields for debugging And ssl/tls status
    Private debug As Boolean = False
    Private ssl As Boolean = False

    'String (as set up on the PayWeb 3 config page in the PayGate Back Office )
    Private encryptionKey As String

    'Series of getter / setter functions
    Public Function isDebug() As Boolean
        Return Me.debug
    End Function

    Public Sub setDebug(ByVal debug As Boolean)
        Me.debug = debug
    End Sub

    Public Function isSsl() As Boolean
        Return Me.ssl
    End Function

    Public Sub setSsl(ByVal ssl As Boolean)
        Me.ssl = ssl
    End Sub

    Public Function getInitiateRequest() As Dictionary(Of String, String)
        Return Me.initiateRequest
    End Function

    Public Sub setInitiateRequest(ByVal postData As Dictionary(Of String, String))
        Me.initiateRequest = postData
    End Sub

    Public Function getQueryRequest() As Dictionary(Of String, String)
        Return Me.queryRequest
    End Function

    Public Sub setQueryRequest(ByVal queryRequest As Dictionary(Of String, String))
        Me.queryRequest = queryRequest
    End Sub

    Public Function getEncryptionKey() As String
        Return Me.encryptionKey
    End Function

    Public Sub setEncryptionKey(ByVal encryptionKey As String)
        Me.encryptionKey = encryptionKey
    End Sub

    '  Returns a description Of the transaction status number passed back from PayGate
    '*
    '* @param int statusNumber
    '* @return string
    Public Function getTransactionStatusDescription(ByVal statusNumber As Integer) As String
        Return Me.transactionStatusArray(statusNumber)
    End Function


    '  Function to generate the checksum To be passed In the initiate Call. Refer To examples On Page 19 Of the PayWeb3 documentation (Version 1.03.2)
    '*
    '* @param Dictionary<string, string> postData
    '* @return string (md5 hash value)
    Public Function generateChecksum(ByVal postData As Dictionary(Of String, String)) As String
        Dim checksum As String = ""

        For Each item In postData

            If item.Value <> "" Then
                checksum += item.Value
            End If
        Next

        checksum += Me.getEncryptionKey()

        If Me.isDebug() Then
            Console.Write("Checksum Source: {0}", checksum)
        End If

        Dim hash = MD5.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes(checksum))
        Dim sb As StringBuilder = New StringBuilder()

        For i As Integer = 0 To hash.Length - 1
            sb.Append(hash(i).ToString("x2"))
        Next

        Return sb.ToString()
    End Function

    'Function to() compare checksums 
    ' * 
    ' * @param Dictionary<string, string> data
    ' * @return bool
    Public Function validateChecksum(ByVal data As Dictionary(Of String, String)) As Boolean
        Dim returnedChecksum As String = data("CHECKSUM")
        data.Remove("CHECKSUM")
        Dim checksum As String = Me.generateChecksum(data)
        Return String.Compare(returnedChecksum, checksum) = 0
    End Function

    '  Function to handle response from initiate request And Set Error Or processRequest As need be
    '*
    '* @return bool
    Public Function handleInitiateResponse() As Boolean
        If Me.initiateResponse.ContainsKey("ERROR") Then
            Me.lastError = Me.initiateResponse("ERROR")
            Me.initiateResponse.Clear()
            Return False
        End If

        Me.processRequest("PAY_REQUEST_ID") = Me.initiateResponse("PAY_REQUEST_ID")
        Me.processRequest("CHECKSUM") = Me.initiateResponse("CHECKSUM")
        Return True
    End Function


    '  Function to handle response from Query request And Set Error As need be
    '*
    '* @return bool
    Public Function handleQueryResponse() As Boolean
        If Me.queryResponse.ContainsKey("ERROR") Then
            Me.lastError = Me.queryResponse("ERROR")
            Me.queryResponse.Clear()
            Return False
        End If

        Return True
    End Function

    '  Function to do post To PayGate To initiate a PayWeb 3 transaction
    '*
    '* @return bool
    Public Function doInitiate() As Boolean
        Me.initiateRequest("CHECKSUM") = Me.generateChecksum(Me.initiateRequest)
        Dim result = Me.doCurlPost(Me.initiateRequest, initiate_url)
        Dim type = result.[GetType]()

        If type.Name = "String" Then
            Dim response = HttpUtility.ParseQueryString(result)

            For Each key In response.AllKeys
                Dim test = response.[Get](key)
                Me.initiateResponse(key) = response.[Get](key)
            Next

            result = Me.handleInitiateResponse()
        End If

        Return result
    End Function

    'Function to do  post to PayGate to query a PayWeb 3 transaction
    '*
    '* @return bool
    Public Function doQuery() As Object
        Me.queryRequest("CHECKSUM") = Me.generateChecksum(Me.queryRequest)
        Dim result = Me.doCurlPost(Me.queryRequest, query_url)
        Dim type = result.[GetType]()

        If type.Name = "String" Then
            Me.queryResponse = New Dictionary(Of String, String)()
            Dim response = HttpUtility.ParseQueryString(result)

            For Each key In response.AllKeys
                Me.queryResponse(key) = response.[Get](key)
            Next

            result = Me.handleQueryResponse()
        End If

        Return result
    End Function

    'function to do actual post to PayGate
    '*
    '* @param Dictionary postData - data to be posted
    '* @param string url - Url to be posted to
    '* @return bool | string
    Public Function doCurlPost(ByVal postData As Dictionary(Of String, String), ByVal url As String) As Object

        Dim client = New HttpClient()
        Dim content = New FormUrlEncodedContent(postData)
        client.BaseAddress = New Uri(url)

        'Check to see if secure connection
        If Not isSsl() Then
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        End If

        Dim response = client.PostAsync(url, content).Result

        If response.IsSuccessStatusCode Then
            Dim responseString = response.Content.ReadAsStringAsync().Result
            Return responseString
        End If

        Return False
    End Function
End Class
