namespace FurnitureMiniCrm.Services
{
    public class ClientModel
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PatronymicName { get; set; }
        public string Fio => $"{LastName} {FirstName} {PatronymicName}".Trim();
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public ClientAddressModel Address { get; set; }
        public string Comments { get; set; }
    }

    public class ClientAddressModel
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string FlatNumber { get; set; }

        public override string ToString()
        {
            var str = $"г.{City}, ул. {Street} {BuildingNumber}";

            if (!string.IsNullOrWhiteSpace(FlatNumber))
                str = $"{str}, кв. {FlatNumber}";

            return str;
        }
    }
}