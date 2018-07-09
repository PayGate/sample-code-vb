@*
    File showing inputs required for a Vault Payment
    Simple format - same functionality
*@

@Code
    Dim pgid = If(Request("payGateId") IsNot Nothing, Request("payGateId"), PayhostSOAP.DEFAULT_PGID)
    Dim encryptionKey = If(Request("encryptionKey") IsNot Nothing, Request("encryptionKey"), PayhostSOAP.DEFAULT_ENCRYPTION_KEY)
    Dim cardNumber = If(Request("cardNumber") IsNot Nothing, Request("cardNumber"), "")
    Dim expiryDate = If(Request("expiryDate") IsNot Nothing, Request("expiryDate"), "")

    Dim queryResponse = ""
    Dim queryRequest = ""

    If Request("btnSubmit") IsNot Nothing Then
        'Create the PayHost client
        Dim payHostt As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Dim cardVaultRequest As PayHost.CardVaultRequestType = PayhostSOAP.makeCardVaultRequest(Request)

        queryRequest = PayhostSOAP.getXMLText(cardVaultRequest)
        Dim sprequest = New PayHost.SingleVaultRequest With {
            .Item = cardVaultRequest
        }
        Dim sprequest1 = New PayHost.SingleVaultRequest1(sprequest)

        Try
            Dim response = payHostt.SingleVault(sprequest1)
            Dim r = TryCast(response.SingleVaultResponse.Item, PayHost.CardVaultResponseType)
            queryResponse = PayhostSOAP.getXMLText(r)
        Catch e As Exception
            Dim err = e.Message
            queryResponse = err
        End Try
    End If
End Code

<html>
<head>
    <title>PayHost - Card Vault</title>
    <style type="text/css">
        label {
            margin-top: 5px;
            display: inline-block;
            width: 150px;
        }
    </style>
</head>
<body>
    <a href="../../../PayHost/singleVault/cardVault/index.vbhtml">Back to Card Vault</a>
    <br>
    <form role="form" class="form-horizontal text-left" action="simple_cardVault.vbhtml" method="post">
        <label for="payGateId">PayGate ID</label>
        <input type="text" name="payGateId" id="payGateId" value="@pgid" />
        <br>
        <label for="encryptionKey" class="col-sm-3 control-label">Encryption Key</label>
        <input class="form-control" type="text" name="encryptionKey" id="encryptionKey" value="@encryptionKey" />
        <br>
        <label for="cardNumber" class="col-sm-3 control-label">Card Number</label>
        <input class="form-control" type="text" name="cardNumber" id="cardNumber" value="@cardNumber" />
        <br>
        <label for="expiryDate" class="col-sm-3 control-label">Card Expiry Date</label>
        <input class="form-control" type="text" name="expiryDate" id="expiryDate" value="@expiryDate" placeholder="MMYYYY" />

        <div class="userDefined">
            <label for="userFields">User Defined</label>
            <input type="text" name="userKey1" id="userKey1" class="userKey" value="" placeholder="Key" />
            <input type="text" name="userField1" id="userField1" class="userField" value="" placeholder="Value" />
        </div>
        <span id="fieldHolder"></span>
        <br>
        <button id="addUserFieldBtn" type="button">Add User Defined Fields</button>
        <br>
        <br>
        <input id="doVaultBtn" type="submit" name="btnSubmit" value="Do Card Vault" />
    </form>
    <label style="vertical-align: top;" for="request">REQUEST:</label>
    <textarea rows="20" cols="100" id="request">@queryRequest</textarea><br>
    <label style="vertical-align: top;" for="response">RESPONSE:</label>
    <textarea rows="20" cols="100" id="response">@queryResponse</textarea>
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