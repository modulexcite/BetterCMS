﻿using System;
using System.Collections.Generic;
using System.Linq;

using BetterCms.Core.DataAccess;
using BetterCms.Core.DataAccess.DataContext;
using BetterCms.Core.Exceptions.DataTier;
using BetterCms.Module.MediaManager.Models;
using BetterCms.Module.MediaManager.Services;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc;

using ServiceStack.ServiceInterface;

using ITagService = BetterCms.Module.Pages.Services.ITagService;

namespace BetterCms.Module.Api.Operations.MediaManager.Images.Image
{
    /// <summary>
    /// Default image CRUD service.
    /// </summary>
    public class ImageService : Service, IImageService
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The file URL resolver.
        /// </summary>
        private readonly IMediaFileUrlResolver fileUrlResolver;

        /// <summary>
        /// The tag service.
        /// </summary>
        private readonly ITagService tagService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="fileUrlResolver">The file URL resolver.</param>
        /// <param name="tagService">The tag service.</param>
        public ImageService(IRepository repository, IUnitOfWork unitOfWork, IMediaFileUrlResolver fileUrlResolver, ITagService tagService)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.fileUrlResolver = fileUrlResolver;
            this.tagService = tagService;
        }

        /// <summary>
        /// Gets the specified image.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>GetImageRequest</c> with an image.
        /// </returns>
        public GetImageResponse Get(GetImageRequest request)
        {
            var model = repository
                .AsQueryable<MediaImage>(media => media.Id == request.ImageId && media.Type == MediaType.Image)
                .Select(media => new ImageModel
                                     {
                                         Id = media.Id,
                                         Version = media.Version,
                                         CreatedBy = media.CreatedByUser,
                                         CreatedOn = media.CreatedOn,
                                         LastModifiedBy = media.ModifiedByUser,
                                         LastModifiedOn = media.ModifiedOn,

                                         Title = media.Title,
                                         Description = media.Description,
                                         Caption = media.Caption,
                                         FileExtension = media.OriginalFileExtension,
                                         FileSize = media.Size,
                                         ImageUrl = media.PublicUrl,
                                         Width = media.Width,
                                         Height = media.Height,
                                         ThumbnailUrl = media.PublicThumbnailUrl,
                                         ThumbnailWidth = media.ThumbnailWidth,
                                         ThumbnailHeight = media.ThumbnailHeight,
                                         ThumbnailSize = media.ThumbnailSize,
                                         IsArchived = media.IsArchived,
                                         FolderId = media.Folder != null && !media.Folder.IsDeleted ? media.Folder.Id : (Guid?)null,
                                         FolderName = media.Folder != null && !media.Folder.IsDeleted ? media.Folder.Title : null,
                                         PublishedOn = media.PublishedOn,
                                         OriginalFileName = media.OriginalFileName,
                                         OriginalFileExtension = media.OriginalFileExtension,
                                         OriginalWidth = media.OriginalWidth,
                                         OriginalHeight = media.OriginalHeight,
                                         OriginalSize = media.OriginalSize,
                                         OriginalUrl = media.PublicOriginallUrl,

                                         FileUri = media.FileUri.ToString(),
                                         IsUploaded = media.IsUploaded,
                                         IsTemporary = media.IsTemporary,
                                         IsCanceled = media.IsCanceled,
                                         OriginalUri = media.OriginalUri.ToString(),
                                         ThumbnailUri = media.ThumbnailUri.ToString()
                                     })
                .FirstOne();

            model.ImageUrl = fileUrlResolver.EnsureFullPathUrl(model.ImageUrl);
            model.ThumbnailUrl = fileUrlResolver.EnsureFullPathUrl(model.ThumbnailUrl);
            model.OriginalUrl = fileUrlResolver.EnsureFullPathUrl(model.OriginalUrl);

            IList<TagModel> tags;
            if (request.Data.IncludeTags)
            {
                tags =
                    repository.AsQueryable<MediaTag>(mediaTag => mediaTag.Media.Id == request.ImageId && !mediaTag.Tag.IsDeleted)
                              .OrderBy(mediaTag => mediaTag.Tag.Name)
                              .Select(media => new TagModel
                                      {
                                          Id = media.Tag.Id,
                                          Version = media.Tag.Version,
                                          CreatedBy = media.Tag.CreatedByUser,
                                          CreatedOn = media.Tag.CreatedOn,
                                          LastModifiedBy = media.Tag.ModifiedByUser,
                                          LastModifiedOn = media.Tag.ModifiedOn,

                                          Name = media.Tag.Name
                                      })
                              .ToList();
            }
            else
            {
                tags = null;
            }

            return new GetImageResponse
                       {
                           Data = model,
                           Tags = tags
                       };
        }

        /// <summary>
        /// Replaces the image or if it doesn't exist, creates it.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>PutImageResponse</c> with a image id.
        /// </returns>
        public PutImageResponse Put(PutImageRequest request)
        {
            var mediaImage = repository.AsQueryable<MediaImage>().FirstOrDefault(tag1 => tag1.Id == request.ImageId);

            var createImage = mediaImage == null;
            if (createImage)
            {
                mediaImage = new MediaImage { Id = request.ImageId, Type = MediaType.Image };
            }
            else if (request.Data.Version > 0)
            {
                mediaImage.Version = request.Data.Version;
            }

            unitOfWork.BeginTransaction();

            if (!createImage)
            {
                var historyitem = mediaImage.Clone();
                historyitem.Original = mediaImage;
                repository.Save(historyitem);
            }

            mediaImage.Title = request.Data.Title;
            mediaImage.Description = request.Data.Description;
            mediaImage.Caption = request.Data.Caption;
            mediaImage.OriginalFileExtension = request.Data.OriginalFileExtension;
            mediaImage.Size = request.Data.FileSize;
            mediaImage.PublicUrl = request.Data.ImageUrl;
            mediaImage.Width = request.Data.Width;
            mediaImage.Height = request.Data.Height;
            mediaImage.PublicThumbnailUrl = request.Data.ThumbnailUrl;
            mediaImage.ThumbnailWidth = request.Data.ThumbnailWidth;
            mediaImage.ThumbnailHeight = request.Data.ThumbnailHeight;
            mediaImage.ThumbnailSize = request.Data.ThumbnailSize;
            mediaImage.IsArchived = request.Data.IsArchived;
            mediaImage.Folder = request.Data.FolderId.HasValue && !request.Data.FolderId.Value.HasDefaultValue()
                                    ? repository.AsProxy<MediaFolder>(request.Data.FolderId.Value)
                                    : null;
            mediaImage.PublishedOn = request.Data.PublishedOn;
            mediaImage.OriginalFileName = request.Data.OriginalFileName;
            mediaImage.OriginalFileExtension = request.Data.OriginalFileExtension;
            mediaImage.OriginalWidth = request.Data.OriginalWidth;
            mediaImage.OriginalHeight = request.Data.OriginalHeight;
            mediaImage.OriginalSize = request.Data.OriginalSize;
            mediaImage.PublicOriginallUrl = request.Data.OriginalUrl;

            mediaImage.FileUri = new Uri(request.Data.FileUri);
            mediaImage.IsUploaded = request.Data.IsUploaded;
            mediaImage.IsTemporary = request.Data.IsTemporary;
            mediaImage.IsCanceled = request.Data.IsCanceled;
            mediaImage.OriginalUri = new Uri(request.Data.OriginalUri);
            mediaImage.ThumbnailUri = new Uri(request.Data.ThumbnailUri);

            repository.Save(mediaImage);

            IList<Tag> newTags = null;
            if (request.Data.Tags != null)
            {
                tagService.SaveMediaTags(mediaImage, request.Data.Tags, out newTags);
            }

            unitOfWork.Commit();

            // Fire events.
            Events.RootEvents.Instance.OnTagCreated(newTags);
            if (createImage)
            {
                Events.MediaManagerEvents.Instance.OnMediaFileUploaded(mediaImage);
            }
            else
            {
                Events.MediaManagerEvents.Instance.OnMediaFileUpdated(mediaImage);
            }

            return new PutImageResponse
                       {
                           Data = mediaImage.Id
                       };
        }

        /// <summary>
        /// Deletes the specified image.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>DeleteImageResponse</c> with success status.
        /// </returns>
        public DeleteImageResponse Delete(DeleteImageRequest request)
        {
            if (request.Data == null || request.ImageId.HasDefaultValue())
            {
                return new DeleteImageResponse { Data = false };
            }

            var itemToDelete = repository
                .AsQueryable<MediaImage>()
                .Where(p => p.Id == request.ImageId)
                .FirstOne();

            if (request.Data.Version > 0 && itemToDelete.Version != request.Data.Version)
            {
                throw new ConcurrentDataException(itemToDelete);
            }

            repository.Delete(itemToDelete);
            unitOfWork.Commit();

            Events.MediaManagerEvents.Instance.OnMediaFileDeleted(itemToDelete);

            return new DeleteImageResponse { Data = true };
        }
    }
}