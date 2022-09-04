import { Home } from "./components/Home";
import {Settings} from "./components/Settings";
import {Movies} from "./components/Movies";
import {Tasks} from "./components/Tasks";
import {TvShows} from "./components/TvShows";
// import {Series} from "./components/Series";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/tasks',
    element: <Tasks />
  },{
    path: '/settings',
    element: <Settings/>
  },{
    path: '/movies',
    element: <Movies/>
  },{
    path: '/tvshows',
    element: <TvShows/>
  }
];

export default AppRoutes;
