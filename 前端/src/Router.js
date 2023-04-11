import React from 'react';
import {BrowserRouter, Route, Routes} from 'react-router-dom';
import StudentsHomePage from './Students/StudentsHomePage';
import StudentsLogin from './Students/StudentsLogin';
import ChangePassword from './Students/ChangePassword';
import TeacherChangePassword from './Teachers/ChangePassword';
import AdminChangePassword from './Admins/ChangePassword';
import TeachersLogin from './Teachers/TeachersLogin';
import TeachersHomePage from './Teachers/TeachersHomePage';
import AdminsHomePage from './Admins/AdminsHomePage';
import AdminsLogin from './Admins/AdminsLogin';
import OneSchedule from "./Teachers/Schedule";
import SelectSchedule from './Admins/SelectSchedule';
import NotFound from './component/NotFound'

const BasicRoute = () => (
    <BrowserRouter>
    <Routes>
    <Route exact path="/students" element={<StudentsHomePage/>} />
    <Route exact path="/students/log" element={<StudentsLogin/>} />
    <Route exact path="/students/changePassword" element={<ChangePassword/>}/>

    <Route exact path="/teachers/log" element={<TeachersLogin/>} />
    <Route exact path="/teachers" element={<TeachersHomePage/>} />
    <Route exact path="/teachers/schedule" element={<OneSchedule/>} />
    <Route exact path="/teachers/changePassword" element={<TeacherChangePassword/>}/>
    
    <Route exact path="/admins/log" element={<AdminsLogin/>} />
    <Route exact path="/admins" element={<AdminsHomePage/>} />
    <Route exact path="/admins/selectSchedule" element={<SelectSchedule/>} />
    <Route exact path="/admins/changePassword" element={<AdminChangePassword/>}/>
    <Route path='*' element={<NotFound/>}></Route>
    </Routes>
    </BrowserRouter>
    
);


export default BasicRoute;