import NativeRPC from '@token-team/native-rpc-h5';

type AppInfo = {
  appId: string;
  appName: string;
  appVersion: 1;
  systemVersion: 1;
};

type StudentProfile = {
  name: string;
  cardId: string;
  studentId: string;
  college: string;
  eduLevel: string;
  hasProfile: boolean;
}

export  async function getAppInfo(): Promise<AppInfo> {
  const res = await NativeRPC.call<AppInfo>("app.info");
  return res;
}

export async function getStudentProfile(): Promise<StudentProfile> {
  const res = await NativeRPC.call<StudentProfile>("student.profile");
  return res;
}