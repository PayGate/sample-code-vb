@*
    Inputs required to delete vault
    Simple format
*@

@Code
    Dim pgid = If(Request("payGateId") IsNot Nothing, Request("payGateId"), PayhostSOAP.DEFAULT_PGID)
    Dim encryptionKey = If(Request("encryptionKey") IsNot Nothing, Request("encryptionKey"), PayhostSOAP.DEFAULT_ENCRYPTION_KEY)
    Dim vaultId = If(Request("vaultId") IsNot Nothing, Request("vaultId"), "")

    Dim queryText = ""
    Dim responseText = ""

    If Request("btnSubmit") IsNot Nothing Then
        'Create the PayHost client
        Dim payHost As PayHost.PayHOST = New PayHost.PayHOSTClient("PayHOSTSoap11")
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Dim deleteVaultRequest As PayHost.DeleteVaultRequestType = PayhostSOAP.makeDeleteVaultRequest(Request)

        queryText = PayhostSOAP.getXMLText(deleteVaultRequest)

        Dim sprequest = New PayHost.SingleVaultRequest With {
    .Item = deleteVaultRequest
}
        Dim sprequest1 = New PayHost.SingleVaultRequest1(sprequest)

        Try
            Dim response = payHost.SingleVault(sprequest1)
            Dim r = TryCast(response.SingleVaultResponse.Item, PayHost.DeleteVaultResponseType)

            responseText = PayhostSOAP.getXMLText(r)

            'Dim keys = r.Status.[GetType]().GetProperties()
            'For Each key In keys
            '    If (Not (r.Status.GetType.GetProperty(key.Name).GetValue(r.Status, Nothing)) Is Nothing) Then
            '        responseText = (responseText _
            '                    + (key.Name + (": " _
            '                    + (r.Status.GetType.GetProperty(key.Name).GetValue(r.Status, Nothing) + Environment.NewLine))))
            '    End If

            'Next
        Catch ex As Exception
            Dim err = ex.Message
            responseText = err
        End Try

    End If

End Code

<html>
<head>
    <title>PayHost - Delete Vault</title>
    <style type="text/css">
        label {
            margin-top: 5px;
            display: inline-block;
            width: 150px;
        }
    </style>
</head>
<body>
    <a href="../../../PayHost/singleVault/deleteVault/index.vbhtml">Back to Delete Vault</a>
    <br>
    <form role="form" class="form-horizontal text-left" action="simple_deleteVault.vbhtml" method="post">
        <label for="payGateId">PayGate ID</label>
        <input type="text" name="payGateId" id="payGateId" value="@pgid" />
        <br>
        <label for="encryptionKey" class="col-sm-3 control-label">Encryption Key</label>
        <input class="form-control" type="text" name="encryptionKey" id="encryptionKey" value="@encryptionKey" />
        <br>
        <label for="vaultId" class="col-sm-3 control-label">Vault ID</label>
        <input class="form-control" type="text" name="vaultId" id="vaultId" value="@vaultId" placeholder="required" />
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
        <br>
        <input id="doVaultBtn" type="submit" name="btnSubmit" value="Do Delete Vault" />
    </form>
    <label style="vertical-align: top;" for="request">REQUEST:</label>
    <textarea rows="20" cols="100" id="request">@queryText</textarea><br>
    <label style="vertical-align: top;" for="response">RESPONSE:</label>
    <textarea rows="20" cols="100" id="response">@responseText</textarea>
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
