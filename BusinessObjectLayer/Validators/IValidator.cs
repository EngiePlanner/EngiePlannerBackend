namespace BusinessObjectLayer.Validators
{
    public interface IValidator<E>
    {
        void Validate(E e);
        void ValidateCustom(E e);
    }
}
