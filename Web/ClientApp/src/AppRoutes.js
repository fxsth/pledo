import { Home } from "./components/Home";
import {Movies} from "./components/Movies";
import {Settings} from "./components/Settings";
import {Downloads} from "./components/Downloads";
import {Playlists} from "./components/Playlists";
import React from "react";
import {TvShows} from "./components/TvShows";

const AppRoutes = [
  {
    index: true,
    element: <Home/>
  },{
    path: '/settings',
    element: <Settings/>
  },{
    path: '/downloads',
    element: <Downloads />
  },{
    path: '/movies',
    element: <Movies/>
  },{
    path: '/tvshows',
    element: <TvShows/>
  },{
    path: '/playlists',
    element: <Playlists/>
  }
];

export default AppRoutes;
