using UnityEngine;

namespace Script.Client.Mono.Utils.Login {
    /// <summary>
    ///     A little util class which helps to acess user credentials.
    /// </summary>
    public class UserCredentials : MonoBehaviour {
        private static string _username = "";
        private static string _password = "";
        private static string _email = "";
        private static string _gender = "";
        private static string _buildingsColor = "";


        //---------------------------------------------------//
        //---------------------------------------------------//
        //---------------------------------------------------//


        public static string GetUsername() { return _username; }

        public static void SetUsername(string username) { UserCredentials._username = username; }


        //---------------------------------------------------//
        //---------------------------------------------------//
        //---------------------------------------------------//


        public static string GetEmail() { return _email; }

        public static void SetEmail(string email) { UserCredentials._email = email; }


        //---------------------------------------------------//
        //---------------------------------------------------//
        //---------------------------------------------------//


        public static void SetPassword(string password) { UserCredentials._password = password; }

        public static string GetPassword() { return _password; }

        //---------------------------------------------------//
        //---------------------------------------------------//
        //---------------------------------------------------//


        public static void SetGender(string gender) { UserCredentials._gender = gender; }

        public static string GetGender() { return _gender; }


        //---------------------------------------------------//
        //---------------------------------------------------//
        //---------------------------------------------------//


        public static void SetBuildingsColor(string buildingsColor) { UserCredentials._buildingsColor = buildingsColor; }

        public static string GetBuildingsColor() { return _buildingsColor; }
    }
}