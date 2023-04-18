import React from "react";
import { Card, CardBody } from "reactstrap";
import { Link } from "react-router-dom";

// ...


const Video = ({ video }) => {
    return (
        <Card >
            <Link to={`/users/${video.UserProfileId}`}>
            <p className="text-left px-2">Posted by: {video.userProfile.name}</p>
            </Link>
            <CardBody>
                <iframe className="video"
                    src={video.url}
                    title="YouTube video player"
                    frameBorder="0"
                    allow="accelerometer; autoplay; clipboard-write; encrypted-media, gyroscope, picture-in-picture"
                    allowFullScreen 
                />
                <p>
                    <strong>{video.title}</strong>
                </p>
                <p>
                    {video.description}
                </p>
                <ul>
                    {video.comments ? (
                        video.comments.map((c) => <li key={c.Id}>{c.message}</li>)
                    ) : null}
                </ul>
                <Link to={`/videos/${video.id}`}>
                    <strong>{video.title}</strong>
                </Link>

            </CardBody>
        </Card>
    );
};

export default Video;
