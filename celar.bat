@echo on   
color 2f   
mode con: cols=80 lines=25   
@REM   
@echo ��������SVN�ļ������Ժ�......   

  
for /r . %%a in (.) do @if exist "%%a\*.pdb" del /s /f "%%a\*.pdb"  
@echo ������ϣ�����   
@pause  