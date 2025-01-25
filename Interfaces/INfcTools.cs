
namespace DimensionalTag
{
    public interface INfcTools
    {
        /// <summary>
        /// For overwritting existing data.
        /// </summary>
        /// <param name="confirm"></param>
        /// <returns></returns>
        void CanOverWrite(bool confirm);



        /// <summary>
        /// Event for when a tag is read.
        /// </summary>
        event EventHandler<NfcTagEventArgs>? NfcTagEvent;

        /// <summary>
        /// Sets the tag to be written.
        /// </summary>
        /// <param name="toyTag">Tag to write.</param>
        /// <returns>Status of task.</returns>
        Task<ProgressStatus> SendToWrite(ToyTag toyTag);

        /// <summary>
        /// For erasing existing tag data.
        /// </summary>
        /// <param name="confirm"></param>
        /// <returns></returns>
        Task<ProgressStatus> EraseIt(bool confirm);

        /// <summary>
        /// Cancels write by emptying the tag.
        /// </summary>
        void WriteCardCancel();
    }
}
