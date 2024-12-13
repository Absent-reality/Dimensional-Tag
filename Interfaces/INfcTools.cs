
namespace DimensionalTag
{
    public interface INfcTools
    {
        /// <summary>
        /// Event for when a tag is read.
        /// </summary>
        event EventHandler<NfcTagEventArgs>? NfcTagEvent;

        /// <summary>
        /// Sets the tag to be written.
        /// </summary>
        /// <param name="toyTag">Tag to write.</param>
        void SendToWrite(ToyTag toyTag);

        /// <summary>
        /// Cancels write by emptying the tag.
        /// </summary>
        void WriteCardCancel();
    }
}
