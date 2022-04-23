namespace BusinessObjectLayer.Validators
{
    public interface IValidator<E>
    {
        void Validate(E e);
    }
}
