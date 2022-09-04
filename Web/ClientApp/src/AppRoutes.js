import { Home } from "./components/Home";
import {Movies} from "./components/Movies";
import {Tasks} from "./components/Tasks";
import {TvShows} from "./components/TvShows";
// import {Series} from "./components/Series";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },{
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
