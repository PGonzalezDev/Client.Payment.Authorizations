namespace Client.Payments.Authorizations.Context;

public static class ClientSeedData
{
    public static Models.Client[] GetClientSeedData()
    {
        return new Models.Client[]
        {
                new Models.Client()
                {
                    Id = new Guid("2e152826-43d5-4302-a2fc-bba3ac58366f"),
                    Name = "M.Pays",
                    Email = "mpays@mail.com",
                    AuthorizationMode = (int)Models.ClientAuthModeEnum.APPROVE_AND_CONFIRM
                },
                new Models.Client()
                {
                    Id = new Guid("c4c24254-b36d-490d-84d7-32d26556dc64"),
                    Name = "RappyPayments",
                    Email = "rpayments@mail.com",
                    AuthorizationMode = (int)Models.ClientAuthModeEnum.APPROVE
                }
        };
    }
}
