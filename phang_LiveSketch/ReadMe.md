프로젝트 제목 : 포항등대박물관 라이브 스케치
제작 : 개인
제작 기간 : 2022/03 ~ 2022/03
역할 : 스캐너 관련 프로그램 설정 및 제작, 벽면 프로그램 제작

사용언어
- python & opencv
- unity C#

프로젝트 소개 : 
사용자가 마커가 들어있는 a4용지를 색칠한뒤
설치된 스캐너의 버튼을 누르면 스캔을 하고, python프로그램으로 마커를 판별후 벽면pc에 스캔받은 사진을 저장,
벽면 프로그램에 id값을 넘기게된다.
그후 벽면프로그램에서 받은 id값에 따라 오브젝트를 생성한다.
그후 사용자가 벽면에 생성된 캐릭터를 터치시 오브젝트의 애니메이션이 재생된다.

어려웠던 점/ 해결방법 : 
화면 사이즈를 벽면과 같이 맞춰달라는 요청이 있어서
화면 해상도를 벽면 실사이즈에 맞춘뒤 3840x1200 으로 맞추었다.
카메라를 2개를 두고 메인카메라는 실제 화면에 보여주고 다른 카메라는 벽면 사이즈에 맞춘 해상도를 틀어주었다.
라이다 터치를 사용하였는데 비율이 다르기에 비율에 맞춰 터치를 작업하였다.

성과 : 
기존 tcp통신을 사용하여 라이브스케치 프로그램을 만들었는데
특정한 경우 통신이 끊기게 되었는데, udp로 교체후 통신이 끊기는 문제가 없어졌다.

아쉬운점 : 
멀티캐스트를 적용하지 못하여 멀티통신을 잘 작업하지 못하였다.