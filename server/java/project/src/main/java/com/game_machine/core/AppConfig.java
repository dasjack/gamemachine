package com.game_machine.core;


public class AppConfig {
	
	public static class Handlers {
		
		private static String auth;
		private static String team;
		
		public static String getAuth() {
			return auth;
		}
		public static void setAuth(String auth) {
			Handlers.auth = auth;
		}
		public static String getTeam() {
			return team;
		}
		public static void setTeam(String team) {
			Handlers.team = team;
		}
		
		
	}
}
