import clr
import sys
clr.AddReference("PhysX_test2")
from PhysX_test2 import *
from System import *
from System.Math import *

# ���������� ����������, ��������� �� ���������� C#
null = MyDynamics.__null
false = MyDynamics.__false
true = MyDynamics.__true

# �������������� dll-��
# sys.path.Add(GameConfiguration.AppPath)
# clr.AddReferenceToFile("some.dll")


# �������� ����������� �������
# ��� ������ �� �������
# ���������� ������������ �������, ����������� �����, ��� ��� �������� �� ��������� ����� ��������
def Ex(obj) : SE.Instance.Execute(obj)
def ExScript(obj) : SE.Instance.ExScript(obj)