﻿namespace azure_functions.Shared
{
    public class VideoMetadata
    {
        public required string Title { get; init; }
        public required string BlobName { get; init; }
        public required Uri DownloadUri { get; init; }
    }
}
