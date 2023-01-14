import { Home } from "./components/Home";
import {Movies} from "./components/Movies";
import {SyncButton} from "./components/SyncButton";
import {TvShows} from "./components/TvShows";
import {Settings} from "./components/Settings";
import {Downloads} from "./components/Downloads";

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
  }
];

export default AppRoutes;
