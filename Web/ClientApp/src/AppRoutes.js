import { Home } from "./components/Home";
import {Movies} from "./components/Movies";
import {Tasks} from "./components/Tasks";
import {TvShows} from "./components/TvShows";
import {Settings} from "./components/Settings";

const AppRoutes = [
  {
    index: true,
    element: <Home/>
  },{
    path: '/settings',
    element: <Settings/>
  },,{
    path: '/tasks',
    element: <Tasks />
  },{
    path: '/movies',
    element: <Movies/>
  },{
    path: '/tvshows',
    element: <TvShows/>
  }
];

export default AppRoutes;
