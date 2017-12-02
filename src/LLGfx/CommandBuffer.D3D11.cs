namespace LLGfx
{
    partial class CommandBuffer
    {
        private readonly CommandQueue _parent;

        internal override string PlatformGetDebugName() => null;
        internal override void PlatformSetDebugName(string value) { }

        internal CommandBuffer(GraphicsDevice graphicsDevice, CommandQueue parent)
            : base(graphicsDevice)
        {
            _parent = parent;
        }

        private CommandEncoder PlatformGetCommandEncoder(RenderPassDescriptor renderPassDescriptor)
        {
            return new CommandEncoder(
                GraphicsDevice,
                GraphicsDevice.Device.ImmediateContext,
                renderPassDescriptor);
        }

        private void PlatformCommit() { }

        private void PlatformCommitAndPresent(SwapChain swapChain)
        {
            PlatformCommit();

            swapChain.Present();
        }
    }
}