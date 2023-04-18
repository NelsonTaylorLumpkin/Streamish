

import React, { useEffect, useState } from "react";
// import User from "./User";
import { getAllVideos, searchVideos } from "../modules/videoManager"
import { useParams } from "react-router-dom";
import { getUser } from "../modules/UserManager";

const UserVideos = () => {
    const [videos, setVideos] = useState([]);
    const [user, setUser] = useState({});
    const [searchTerm, setSearchTerm] = useState([]);
    const { id } = useParams()

    const getUserVideos = () => {
        getUser(id).then(user => setVideos(user.videos));
    };

    const getUserInfo = () => {
        getUser(id).then(user => setUser(user));
    };

    const handleSearchSubmit = (event) => {
        event.preventDefault();
        searchVideos(searchTerm)
            .then(videos => setSearchedUserVideos(videos))
            .then(setVideos(searchedUserVideos.filter((v) => v.userProfileId == id)))
        if (searchTerm == "") {
            getUserVideos()
        };
    };

    useEffect(() => {
        getUserInfo();
        getUserVideos();
    }, []);

    return (

        <>
        <div className="header">
            <div className="text-left px-2"><h2>{user.name}'s Posts</h2></div>
        </div>
        <div className="container-top">
            <div className="search-container">
                <form onSubmit={(event) => handleSearchSubmit(event)}>
                    <input
                        id="search-value"
                        type="text"
                        onChange={(event) => setSearchTerm(event.target.value)} />
                    <br></br>
                    <input 
                        className="search-button"
                        id="search-submit"
                        type="submit"
                        value=" Search " />

                </form>
            </div>

        </div>
        <div className="list-container">
            <div className="row justify-content-center">
                {videos.map((video) => (
                    <user video={video} key={video.id} />
                    
                ))}
            </div>
        </div>
        </>
    );
};

export default UserVideos;
