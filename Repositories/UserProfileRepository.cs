using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;
using System.Collections.Generic;

namespace Streamish.Repositories
{

    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                      SELECT Id, Name, Email, ImageUrl, DateCreated
                      FROM UserPRofile
                      ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var users = new List<UserProfile>();
                        while (reader.Read())
                        {
                            users.Add(new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "UserProfileId"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                            });
                        }

                        return users;
                    }
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                      SELECT Id, Name, Email, ImageUrl, DateCreated
                      FROM UserPRofile
                      WHERE Id = @Id
                      ";


                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        UserProfile user = null;
                        if (reader.Read())
                        {
                            user = new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),

                            };
                        }

                        return user;
                    }
                }
            }
        }

        public void Add(UserProfile user
            )
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile (Name, Email, DateCreated, ImageUrl)
                        OUTPUT INSERTED.ID
                        VALUES (@Name, @Email, @DateCreated, @ImageUrl)";

                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);


                    user.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile userProfile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile SET Name = @Name, Email = @Email,
                        ImageUrl = @ImageUrl, DateCreated = @DateCreated
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Name", userProfile.Name);
                    DbUtils.AddParameter(cmd, "@Email", userProfile.Email);
                    DbUtils.AddParameter(cmd, "@ImageUrl", userProfile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@DateCreated", userProfile.DateCreated);


                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                      DELETE FROM Comment WHERE UserProfileId = @Id;
                                      DELETE FROM Video WHERE UserProfileId = @Id;
                                      DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public UserProfile GetUserByIdWithVideosAndComments(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT up.Id AS UserId, up.Name, up.Email, up.DateCreated AS UserProfileDateCreated, up.ImageUrl AS UserProfileImageUrl,
                                        v.Id AS VideoId, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId As VideoUserProfileId,
                                        c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId, c.VideoId AS CommentVideoId
                                        FROM UserProfile up
                                        JOIN Video v ON v.UserProfileId = up.Id
                                        LEFT JOIN Comment c on c.VideoId = v.id
                                        WHERE up.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile user = null;
                        while (reader.Read())
                        {
                            var userId = DbUtils.GetInt(reader, "UserId");
                            if (user == null)
                            {
                                user = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserId"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    Videos = new List<Video>(),
                                    Comments = new List<Comment>()
                                };
                            }
                            if (DbUtils.IsNotDbNull(reader, "VideoId"))
                            {
                                user.Videos.Add(new Video()
                                {
                                    Id = DbUtils.GetInt(reader, "VideoId"),
                                    Title = DbUtils.GetString(reader, "Title"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                    Description = DbUtils.GetString(reader, "Description"),
                                    Url = DbUtils.GetString(reader, "Url"),
                                    UserProfileId = DbUtils.GetInt(reader, "VideoUserProfileId"),
                                });
                            }
                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                user.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = DbUtils.GetInt(reader, "CommentVideoId"),
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                                });
                            }

                        }
                        return user;
                    }
                }
            }
        }

        //public List<Video> GetAllWithComments()
        //{
        //    using (var conn = Connection)
        //    {
        //        conn.Open();
        //        using (var cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //        SELECT v.Id AS VideoId, v.Title, v.Description, v.Url, 
        //               v.DateCreated AS VideoDateCreated, v.UserProfileId As VideoUserProfileId,

        //               up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
        //               up.ImageUrl AS UserProfileImageUrl,

        //               c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId
        //          FROM Video v 
        //               JOIN UserProfile up ON v.UserProfileId = up.Id
        //               LEFT JOIN Comment c on c.VideoId = v.id
        //     ORDER BY  v.DateCreated
        //    ";

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {

        //                var videos = new List<Video>();
        //                while (reader.Read())
        //                {
        //                    var videoId = DbUtils.GetInt(reader, "VideoId");

        //                    var existingVideo = videos.FirstOrDefault(p => p.Id == videoId);
        //                    if (existingVideo == null)
        //                    {
        //                        existingVideo = new Video()
        //                        {
        //                            Id = videoId,
        //                            Title = DbUtils.GetString(reader, "Title"),
        //                            Description = DbUtils.GetString(reader, "Description"),
        //                            DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
        //                            Url = DbUtils.GetString(reader, "Url"),
        //                            UserProfileId = DbUtils.GetInt(reader, "VideoUserProfileId"),
        //                            UserProfile = new UserProfile()
        //                            {
        //                                Id = DbUtils.GetInt(reader, "VideoUserProfileId"),
        //                                Name = DbUtils.GetString(reader, "Name"),
        //                                Email = DbUtils.GetString(reader, "Email"),
        //                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
        //                                ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
        //                            },
        //                            Comments = new List<Comment>()
        //                        };

        //                        videos.Add(existingVideo);
        //                    }

        //                    if (DbUtils.IsNotDbNull(reader, "CommentId"))
        //                    {
        //                        existingVideo.Comments.Add(new Comment()
        //                        {
        //                            Id = DbUtils.GetInt(reader, "CommentId"),
        //                            Message = DbUtils.GetString(reader, "Message"),
        //                            VideoId = videoId,
        //                            UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
        //                        });
        //                    }
        //                }

        //                return videos;
        //            }
        //        }
        //    }
        //}
        //public Video GetByIdWithVideos(int Id)
        //{
        //    using (var conn = Connection)
        //    {
        //        conn.Open();
        //        using (var cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT up.Id, up.[Name], up.Email, up.ImageUrl, up.DateCreated
        //                                v.Id AS VideoId,
        //                                v.Title, v.Description, v.DateCreated AS VideoDateCreated,
        //                                v.Url, v.UserProfileId AS VideoUserProfileId, 
        //                                c.Id, AS CommentId, v.VideoId AS CommentVideoId, c.Message, c.UserProfileId AS CommentUserProfileId
        //                                FROM UserProfile up
        //                                JOIN Video v  ON v.UserProfileId = up.Id
        //                                LEFT JOIN Comment c on c.VideoId = v.Id
        //                                WHERE up.Id =@Id";
        //            DbUtils.AddParameter(cmd, "@id", Id);

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                UserProfile userProfile = null;
        //                while (reader.Read())
        //                {
        //                    if (userProfile == null)
        //                    {
        //                        userProfile = new UserProfile()
        //                        {
        //                            Id = DbUtils.GetInt(reader, "Id"),
        //                            Name = DbUtils.GetString(reader, "Name"),
        //                            Email = DbUtils.GetString(reader, "Email"),
        //                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
        //                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
        //                            Videos = new List<Video>(),
        //                            Comments = new List<Comment>()

        //                        };
        //                    }
        //                    if (DbUtils.IsNotDbNull(reader, "VideoId"))
        //                    {
        //                        var videoId = DbUtils.GetInt(reader, "VideoId");
        //                        var existingVideo = userProfile.Videos.FirstOrDefault(p => p.Id == videoId);

        //                        {
        //                            existingVideo = new Video()
        //                            {
        //                                Id = videoId,
        //                                Title = DbUtils.GetString(reader, "Title"),
        //                                Description = DbUtils.GetString(reader, "Description"),
        //                                DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
        //                                Url = DbUtils.GetString(reader, "Url"),
        //                                UserProfileId = DbUtils.GetInt(reader, "VideoUserProfileId"),
        //                                Comments = new List<Comment>()
        //                            };
        //                            userProfile.Videos.Add(existingVideo);
        //                        }
        //                        if (DbUtils.IsNotDbNull(reader, "CommentId"))
        //                        {

        //                            existingVideo.Comments.Add(new Comment()
        //                            {
        //                                Id = DbUtils.GetInt(reader, "CommentId"),
        //                                Message = DbUtils.GetString(reader, "Message"),
        //                                VideoId = videoId,
        //                                UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
        //                            });

        //                        }
        //                    }
        //                }
        //                return userProfile;
        //            }
        //        }
        //    }
        //}
        //public List<Video> Search(string criterion, bool sortDescending)
        //{
        //    using (var conn = Connection)
        //    {
        //        conn.Open();
        //        using (var cmd = conn.CreateCommand())
        //        {
        //            var sql = @"
        //                      SELECT v.Id, v.Title, V.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId,
        //                             up.Name, up.Email, up.DateCreated AS UserProfileCreated,
        //                             up.ImageUrl AS UserProfileImageUrl

        //                      FROM Video v
        //                             JOIN UserProfile up ON v.UserProfileId = up.Id
        //                      WHERE v.Title LIKE @Criterion OR v.Description LIKE @Criterion";

        //            if (sortDescending)
        //            {
        //                sql += " ORDER BY v.DateCreated DESC";
        //            }
        //            else
        //            {
        //                sql += " ORDER BY v.DateCreated";
        //            }
        //            cmd.CommandText = sql;
        //            DbUtils.AddParameter(cmd, "@Criterion", $"%{criterion}%");
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                var videos = new List<Video>();
        //                while (reader.Read())
        //                {
        //                    videos.Add(new Video()
        //                    {
        //                        Id = DbUtils.GetInt(reader, "Id"),
        //                        Title = DbUtils.GetString(reader, "Title"),
        //                        Description = DbUtils.GetString(reader, "Description"),
        //                        DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
        //                        Url = DbUtils.GetString(reader, "Url"),
        //                        UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
        //                        UserProfile = new UserProfile()
        //                        {
        //                            Id = DbUtils.GetInt(reader, "UserProfileId"),
        //                            Name = DbUtils.GetString(reader, "Name"),
        //                            Email = DbUtils.GetString(reader, "Email"),
        //                            DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
        //                            ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),


        //                        },
        //                    });
        //                }
        //                return videos;

        //            }
        //        }
        //    }
        //}




    }
}